using UnityEngine;
using System.Collections;

public class AnchorNode : MonoBehaviour {

	public bool blocked;
	public AnchorState anchorState;
	private Material mat;

	public void Initialize(int posX, int posY) {
		mat = GetComponent<Renderer>().material;
	}

	public void SetAnchorState(AnchorState state) {
		anchorState = state;
		switch (state) {
			case AnchorState.None:
				if (blocked) {
					mat.color = Color.red;
				} else {
					mat.color = Color.white;
				}
				break;
			case AnchorState.Active:
				mat.color = Color.green;
				blocked = true;
				break;
			case AnchorState.Debug:
				mat.color = Color.blue;
				break;
			default:
				break;
		}
	}

}