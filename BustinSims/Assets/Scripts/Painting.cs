using UnityEngine;
using System.Collections.Generic;

public class Painting : MonoBehaviour {

	private float segmentLength = 1f;
	private bool canBuild = true;
	private bool animate = true;
	private bool perPiece = false;

	private bool onlyStraight = true;

	private Vector3 startPos;
	private Vector3 nextPos;

	private GameObject wallObject;

	private List<Vector3> pathPoints;
	private WorldController worldController;

	void Start () {
		worldController = GameAccesPoint.worldController;

		pathPoints = new List<Vector3>();

		wallObject = (GameObject)Resources.Load("Prefabs/Wall_Piece");
		if (wallObject != null) {
			segmentLength = wallObject.transform.GetChild(0).GetComponent<MeshFilter>().sharedMesh.bounds.extents.x * 2;
		} else {
			Debug.LogWarning("Prefab not defined");
			canBuild = false;
		}
	}

	private void Update () {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitInfo;
		Vector3 hitPos;

		if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, (1 << 8))) {
			hitPos = hitInfo.point;
			if (canBuild) {
				if (Input.GetMouseButtonDown(0)) {
					startPos = CheckNearbyAnchors(hitPos);
					pathPoints.Add(startPos);
				} else if (Input.GetMouseButton(0) && canBuild) {
					nextPos = hitPos;
					Vector3 dir = nextPos - startPos;
					if (dir.magnitude >= segmentLength) {
						if (onlyStraight) {
							dir = LockToDegrees(dir);
						}
						Vector3 newPoint = startPos + (dir.normalized * segmentLength);
						foreach (AnchorNode a in worldController._anchors) {
							float dist = Vector3.Distance(newPoint, a.transform.position);
							if (dist < 0.5f) {
								newPoint = a.transform.position;
								a.SetAnchorState(AnchorState.Active);

								break;
							}
						}
						if (perPiece) {
							CreateFencePiece(startPos, newPoint);
						} else {
							pathPoints.Add(newPoint);
						}
						startPos = newPoint;
					}
				} else if (Input.GetMouseButtonUp(0)) {
					if (!perPiece) {
						canBuild = false;
						PathDone();
					}
				}
			}
		}
	}

	private Vector3 CheckNearbyAnchors(Vector3 cursorPos) {
		if (worldController._grid != null && worldController._grid.Length > 0) {
			foreach (AnchorNode a in worldController._anchors) {
				float dist = Vector3.Distance(cursorPos, a.transform.position);
				if (dist < 0.5f) {
					a.SetAnchorState(AnchorState.Active);
					return a.transform.position;
				}
			}
		}
		return cursorPos;
	}

	private void CreateFencePiece(Vector3 startPos, Vector3 endPos) {
		float yRot = GetYRotation(startPos, endPos);
		GameObject g = (GameObject)Instantiate(wallObject, startPos, Quaternion.Euler(new Vector3(0, (yRot + 90) + 180, 0)));
		g.transform.localScale = new Vector3(Vector3.Distance(startPos, endPos), g.transform.localScale.y, g.transform.localScale.z);
		if (animate) {
			PopupAnimation pA = g.AddComponent<PopupAnimation>();
			pA.SetInitialScales(g.transform.localScale, 0);
		}
	}

	// TODO: If node at pathpoint and to pathpoint are blocked ignore spawning
	private void PathDone() {
		if (pathPoints.Count < 2)
			return;

		for (int i = 0; i < pathPoints.Count - 1; i++) {
			float yRot = GetYRotation(pathPoints[i], pathPoints[i + 1]);
			GameObject g = (GameObject)Instantiate(wallObject, pathPoints[i], Quaternion.Euler(new Vector3(0, (yRot + 90) + 180, 0)));
			g.transform.localScale = new Vector3(Vector3.Distance(pathPoints[i], pathPoints[i + 1]), 1, 1);
			if (animate) {
				PopupAnimation pA = g.AddComponent<PopupAnimation>();
				pA.SetInitialScales(g.transform.localScale, i);
			}
		}

		for (int i = 1; i < pathPoints.Count; i++) {
			Wall wall = new Wall();
			foreach (TileNode n in worldController._grid) {
				n.SetWall(worldController.GetAnchorFromPos(pathPoints[i - 1]), worldController.GetAnchorFromPos(pathPoints[i]), wall);
			}
		}

		foreach (AnchorNode a in worldController._anchors) {
			a.SetAnchorState(AnchorState.None);
		}

		pathPoints.Clear();
		startPos = Vector3.zero;
		nextPos = Vector3.zero;
		canBuild = true;
	}

	private float GetYRotation(Vector3 v1, Vector3 v2) {
		float dirRads = Mathf.Atan2(v2.x - v1.x, v2.z - v1.z);
		float angle = (dirRads / Mathf.PI) * 180;

		return angle;
	}

	private Vector3 LockToDegrees(Vector3 dir) {
		float rads = Mathf.Atan2(dir.z, dir.x);
		float a = (rads / Mathf.PI) * 180;
		a = Mathf.Round(a / 90) * 90;

		dir.x = Mathf.Cos(Mathf.Deg2Rad * a);
		dir.z = Mathf.Sin(Mathf.Deg2Rad * a);

		return dir;
	}
}

public static class RoomAlgorithm {

	private static List<TileNode> roomNodes;
	public static List<TileNode> IsRoom(TileNode startTile) {
		roomNodes = new List<TileNode>();

		foreach (TileNode n in GameAccesPoint.worldController._grid) {
			n.GetComponent<Renderer>().material.color = Color.white;
			n.nodeState = NodeState.Open;
		}

		CheckTile(startTile);

		foreach (TileNode n in roomNodes) {
			if (n.hiddenEdgeTile) {
				return null;
			}
		}
		return roomNodes;
	}

	private static void CheckTile(TileNode currentTile) {
		currentTile.nodeState = NodeState.Closed;
		if (!roomNodes.Contains(currentTile)) {
			roomNodes.Add(currentTile);
		}
		List<TileNode> reachableNodes = currentTile.GetReachableNeighbours();
		if (reachableNodes.Count > 0) {
			foreach (TileNode n in reachableNodes) {
				if (n.nodeState != NodeState.Closed) {
					CheckTile(n);
				}
			}
		}
	}

}