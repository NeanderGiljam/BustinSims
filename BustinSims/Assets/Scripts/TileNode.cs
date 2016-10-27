using UnityEngine;
using System.Collections.Generic;

public class TileNode : MonoBehaviour {

	public int _posX { get; private set; }
	public int _posY { get; private set; }

	public List<TileNode> _neighbours { get; private set; }
	public Dictionary<AnchorNode, AnchorPosition> _anchors { get; private set; }
	
	public NodeState nodeState = NodeState.Open;

	public bool hiddenEdgeTile;
	private List<Wall> walls = new List<Wall>();

	public void Initialize(int posX, int posY) {
		_posX = posX;
		_posY = posY;
	}

	public void SetWall(AnchorNode firstAnchor, AnchorNode secondAnchor, Wall wall) {
		if (_anchors.ContainsKey(firstAnchor) && _anchors.ContainsKey(secondAnchor)) {
			if (!walls.Contains(wall)) {
				walls.Add(wall);
			}
		}
	}

	public bool HasWall(Wall wall) {
		if (walls.Contains(wall)) {
			return true;
		}
		return false;
	}

	public List<TileNode> GetReachableNeighbours() {
		if (walls.Count <= 0) {
			return _neighbours;
		}

		List<TileNode> reachableNeighbours = new List<TileNode>();
		foreach (TileNode n in _neighbours) {
			if (!ShareWall(n)) {
				reachableNeighbours.Add(n);
			}
		}
		return reachableNeighbours;
	}

	private bool ShareWall(TileNode n) {
		foreach (Wall w in walls) {
			if (n.HasWall(w)) {
				return true;
			}
		}
		return false;
	}

	public void SetNeighbours(WorldController worldController) {
		_neighbours = new List<TileNode>();
		for (int dx = -1; dx <= 1; ++dx) {
			for (int dy = -1; dy <= 1; ++dy) {
				if (dx == 0 || dy == 0) {
					if (_posX + dx >= 0 && _posX + dx < worldController._levelWidth) {
						if (_posY + dy >= 0 && _posY + dy < worldController._levelHeight) {
							_neighbours.Add(worldController._grid[_posX + dx, _posY + dy]);
						}
					}
				}
			}
		}
	}

	public void SetAnchors(WorldController worldController) {
		_anchors = new Dictionary<AnchorNode, AnchorPosition>();
		for (int dx = -1; dx <= 1; ++dx) {
			for (int dy = -1; dy <= 1; ++dy) {
				if (dx != 0 || dy != 0) {
					AnchorNode n = worldController.GetAnchorFromPos(new Vector3((_posX + (dx * 0.5f) + 0.5f) - (worldController._levelWidth / 2), 0, (_posY + (dy * 0.5f) + 0.5f) - (worldController._levelHeight / 2)));
					if (n != null) {
						AnchorPosition anchorPos = GetAnchorPosition(n);
						_anchors.Add(n, anchorPos);
					}
				}
			}
		}
	}

	private AnchorPosition GetAnchorPosition(AnchorNode n) {
		float xOffset = transform.position.x - n.transform.position.x;
		float zOffset = transform.position.z - n.transform.position.z;

		if (xOffset > 0) {
			if (zOffset > 0)
				return AnchorPosition.TopRight;
			else
				return AnchorPosition.BottomRight;
		} else {
			if (zOffset > 0)
				return AnchorPosition.TopLeft;
			else
				return AnchorPosition.BottomLeft;
		}
	}

	//private void OnMouseEnter() {
	//	foreach (AnchorNode n in _anchors.Keys) {
	//		n.SetAnchorState(AnchorState.Debug);
	//	}
	//}

	//private void OnMouseExit() {
	//	foreach (AnchorNode n in _anchors.Keys) {
	//		n.SetAnchorState(AnchorState.None);
	//	}
	//}

	private void OnMouseOver() {
		if (Input.GetMouseButtonDown(1)) {
			List<TileNode> roomNodes = RoomAlgorithm.IsRoom(this);
			if (roomNodes != null && roomNodes.Count > 0) {
				GameObject ceillingPrefab = (GameObject)Resources.Load("Prefabs/Ceilling");

				foreach (TileNode n in roomNodes) {
					n.GetComponent<Renderer>().material.color = Color.blue;
					if (ceillingPrefab != null) {
						if (n.transform.childCount <= 0) {
							Instantiate(ceillingPrefab, n.transform.position, Quaternion.identity, n.transform);
						}
					}
				}
			}
		}
	}
}