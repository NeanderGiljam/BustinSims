using UnityEngine;
using System.Collections.Generic;

public class WorldController : MonoBehaviour {

	public TileNode[,] _grid { get; private set; }
	public AnchorNode[,] _anchors { get; private set; }

	public int _levelWidth { get; private set; }
	public int _levelHeight { get; private set; }

	private GameObject floorTilePrefab;

	// TODO: Clean grid and anchor generation
	void Awake() {
		GameAccesPoint.worldController = this;

		_levelWidth = 10;
		_levelHeight = 10;

		_grid = new TileNode[_levelWidth, _levelHeight];
		_anchors = new AnchorNode[_levelWidth - 1, _levelHeight - 1];

		floorTilePrefab = (GameObject)Resources.Load("Prefabs/Floor_Tile");

		SpawnTiles();
		SpawnAnchors();

		foreach (TileNode t in _grid) {
			t.SetNeighbours(this);
			t.SetAnchors(this);
		}
	}

	private void SpawnTiles() {
		if (floorTilePrefab == null)
			return;

		for (int y = 0; y < _levelHeight; y++) {
			for (int x = 0; x < _levelWidth; x++) {
				GameObject g = (GameObject)Instantiate(floorTilePrefab, new Vector3((x + 0.5f) - (_levelWidth / 2), 0, (y + 0.5f) - (_levelHeight / 2)), floorTilePrefab.transform.rotation);
				g.transform.SetParent(transform);
				g.name = "Tile (" + x + "," + y + ")";

				TileNode n = g.AddComponent<TileNode>();
				n.transform.SetParent(transform);
				n.Initialize(x, y);
				_grid[x, y] = n;

				if (x == 0 || y == 0 || x == _levelWidth - 1 || y == _levelHeight - 1) {
					//g.GetComponent<Renderer>().enabled = false;
					n.hiddenEdgeTile = true;
				}
			}
		}
	}

	private void SpawnAnchors() {
		for (int y = 1; y < _levelHeight; y++) {
			for (int x = 1; x < _levelWidth; x++) {
				GameObject g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				g.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
				g.transform.position = new Vector3(x - (_levelHeight / 2), 0, y - (_levelWidth / 2));
				g.name = "Anchor (" + x + "," + y + ")";

				AnchorNode n = g.AddComponent<AnchorNode>();
				n.transform.SetParent(transform);
				n.Initialize(x - 1, y - 1);
				_anchors[x - 1, y - 1] = n;
			}
		}
	}

	public TileNode GetTileFromPos(Vector3 pos) {
		TileNode n = _grid[(int)pos.x + (_levelWidth / 2), (int)pos.z + (_levelHeight / 2)];
		if (n == null) {
			Debug.LogWarning("Could not get tile from pos");
		}
		return n;
	}

	public AnchorNode GetAnchorFromPos(Vector3 pos) {
		AnchorNode result = null;
		foreach (AnchorNode n in _anchors) {
			if (n.transform.position == new Vector3(pos.x, 0, pos.z)) {
				return n;
			}
		}
		return result;
	}
}