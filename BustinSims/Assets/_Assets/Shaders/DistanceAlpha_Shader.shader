// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: commented out 'float4 unity_DynamicLightmapST', a built-in variable
// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable

// Shader created with Shader Forge v1.04 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.04;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:1,uamb:True,mssp:True,lmpd:False,lprd:False,rprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:True,dith:2,ufog:True,aust:False,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.7573529,fgcg:0.2895761,fgcb:0.2895761,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:9,ofsu:9,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:34525,y:32703,varname:node_1,prsc:2|diff-3344-RGB,alpha-7368-OUT;n:type:ShaderForge.SFN_Clamp01,id:23,x:33907,y:33703,varname:node_23,prsc:2|IN-28-OUT;n:type:ShaderForge.SFN_Power,id:28,x:33735,y:33703,varname:node_28,prsc:2|VAL-30-OUT,EXP-1456-OUT;n:type:ShaderForge.SFN_Divide,id:30,x:33377,y:33703,varname:node_30,prsc:2|A-36-OUT,B-6264-OUT;n:type:ShaderForge.SFN_Distance,id:36,x:33617,y:33121,varname:node_36,prsc:2|A-4864-XYZ,B-41-XYZ;n:type:ShaderForge.SFN_ViewPosition,id:41,x:33405,y:33249,varname:node_41,prsc:2;n:type:ShaderForge.SFN_ObjectPosition,id:49,x:33405,y:33121,varname:node_49,prsc:2;n:type:ShaderForge.SFN_Lerp,id:2051,x:34323,y:33441,varname:node_2051,prsc:2|A-426-OUT,B-3228-OUT,T-3357-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3228,x:33405,y:33653,ptovrint:False,ptlb:MinAlpha,ptin:_MinAlpha,varname:node_3228,prsc:2,glob:False,v1:0;n:type:ShaderForge.SFN_OneMinus,id:3357,x:34083,y:33703,varname:node_3357,prsc:2|IN-23-OUT;n:type:ShaderForge.SFN_VertexColor,id:3344,x:33335,y:32699,varname:node_3344,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:1456,x:33405,y:33400,ptovrint:False,ptlb:Falloff,ptin:_Falloff,varname:node_1456,prsc:2,glob:False,v1:20;n:type:ShaderForge.SFN_ValueProperty,id:6264,x:33405,y:33494,ptovrint:False,ptlb:Distance,ptin:_Distance,varname:node_6264,prsc:2,glob:False,v1:5;n:type:ShaderForge.SFN_Vector1,id:426,x:33405,y:33564,varname:node_426,prsc:2,v1:1;n:type:ShaderForge.SFN_If,id:7368,x:34323,y:33298,varname:node_7368,prsc:2|A-36-OUT,B-6264-OUT,GT-426-OUT,EQ-2051-OUT,LT-2051-OUT;n:type:ShaderForge.SFN_FragmentPosition,id:4864,x:33402,y:32979,varname:node_4864,prsc:2;proporder:3228-1456-6264;pass:END;sub:END;*/

Shader "Shader Forge/DistanceAlpha" {
    Properties {
        _MinAlpha ("MinAlpha", Float ) = 0
        _Falloff ("Falloff", Float ) = 20
        _Distance ("Distance", Float ) = 5
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            
            Offset 9, 9
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _LightColor0;
            // float4 unity_LightmapST;
            #ifdef DYNAMICLIGHTMAP_ON
                // float4 unity_DynamicLightmapST;
            #endif
            uniform float _MinAlpha;
            uniform float _Falloff;
            uniform float _Distance;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(2)
                #ifndef LIGHTMAP_OFF
                    float4 uvLM : TEXCOORD3;
                #else
                    float3 shLight : TEXCOORD3;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.vertexColor = v.vertexColor;
                o.normalDir = mul(unity_ObjectToWorld, float4(v.normal,0)).xyz;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 indirectDiffuse = float3(0,0,0);
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                float3 diffuse = (directDiffuse + indirectDiffuse) * i.vertexColor.rgb;
/// Final Color:
                float3 finalColor = diffuse;
                float node_36 = distance(i.posWorld.rgb,_WorldSpaceCameraPos);
                float node_7368_if_leA = step(node_36,_Distance);
                float node_7368_if_leB = step(_Distance,node_36);
                float node_426 = 1.0;
                float node_2051 = lerp(node_426,_MinAlpha,(1.0 - saturate(pow((node_36/_Distance),_Falloff))));
                fixed4 finalRGBA = fixed4(finalColor,lerp((node_7368_if_leA*node_2051)+(node_7368_if_leB*node_426),node_2051,node_7368_if_leA*node_7368_if_leB));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ForwardAdd"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            Offset 9, 9
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd
            #pragma multi_compile_fog
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _LightColor0;
            // float4 unity_LightmapST;
            #ifdef DYNAMICLIGHTMAP_ON
                // float4 unity_DynamicLightmapST;
            #endif
            uniform float _MinAlpha;
            uniform float _Falloff;
            uniform float _Distance;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float4 vertexColor : COLOR;
                LIGHTING_COORDS(2,3)
                #ifndef LIGHTMAP_OFF
                    float4 uvLM : TEXCOORD4;
                #else
                    float3 shLight : TEXCOORD4;
                #endif
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.vertexColor = v.vertexColor;
                o.normalDir = mul(unity_ObjectToWorld, float4(v.normal,0)).xyz;
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float3 diffuse = directDiffuse * i.vertexColor.rgb;
/// Final Color:
                float3 finalColor = diffuse;
                float node_36 = distance(i.posWorld.rgb,_WorldSpaceCameraPos);
                float node_7368_if_leA = step(node_36,_Distance);
                float node_7368_if_leB = step(_Distance,node_36);
                float node_426 = 1.0;
                float node_2051 = lerp(node_426,_MinAlpha,(1.0 - saturate(pow((node_36/_Distance),_Falloff))));
                return fixed4(finalColor * lerp((node_7368_if_leA*node_2051)+(node_7368_if_leB*node_426),node_2051,node_7368_if_leA*node_7368_if_leB),0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
