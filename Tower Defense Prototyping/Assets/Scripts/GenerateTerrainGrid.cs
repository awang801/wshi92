using UnityEngine;
using System.Collections;

public class GenerateTerrainGrid : MonoBehaviour {

	public GameObject block;

	static int mapSizeX = 100;
	static int mapSizeY = 100;

	public GameObject[,] grid = new GameObject[mapSizeX, mapSizeY];

	// Use this for initialization
	void Start () {
		GenerateBoard ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void GenerateBoard()
	{
		for (int x = 0; x < mapSizeX; x++) {
			for (int y = 0; y < mapSizeY; y++) {
				grid [x, y] = ((GameObject)(Instantiate (block, new Vector3 (x, 0f, y), Quaternion.identity)));
				grid [x, y].transform.SetParent (transform);
			}
		}
	}
}
