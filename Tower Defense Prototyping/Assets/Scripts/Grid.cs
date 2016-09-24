using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Grid : NetworkBehaviour {

	public Vector3 gridZeroPoint;

	public Vector2 gridWorldSize; //Set in editor
	public float nodeRadius;
	public LayerMask terrainMask;

	Node[,] grid;

	float nodeDiameter;
	int gridSizeX, gridSizeY;

	void Start()
	{
		gridZeroPoint = transform.position;
		nodeDiameter = nodeRadius * 2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

		CreateGrid ();
	}

	void CreateGrid()
	{
		/*
		 * Generates the array of nodes of size gridSizeX and gridSizeY, stored as "grid"
		 */
		grid = new Node[gridSizeX,gridSizeY];

		for (int x = 0; x < gridSizeX; x++) {
			for (int y = 0; y < gridSizeY; y++) {
				grid[x,y] = new Node(new Vector3(x + gridZeroPoint.x, 0, y + gridZeroPoint.z), x, y);
			}
		}
	}

	void OnDrawGizmos()
	{
		/*
		 * Draws a representation of the grid that is visible only in the editor as a white outline
		 */
		Vector3 box = new Vector3 (gridWorldSize.x, 1, gridWorldSize.y);
		Gizmos.DrawWireCube (transform.position + box * 0.5f, box);
	}

	public Node NodeFromWorldPoint(Vector3 worldPosition)
	{
		// Returns a Node from a world position

		int x = Mathf.CeilToInt (worldPosition.x - gridZeroPoint.x);
		int y = Mathf.CeilToInt (worldPosition.z - gridZeroPoint.z);

		if (x >= gridSizeX && y >= gridSizeY) {
			
			return grid [gridSizeX - 1, gridSizeY - 1];

		} else if (x >= gridSizeX) {
			
			return grid [gridSizeX - 1, y];

		} else if (y >= gridSizeY) {
			
			return grid [x, gridSizeY - 1];

		} else if (x <= 0) {

			return grid [0, y];

		} else if (y <= 0) {

			return grid [x, 0];

		} else {
			return grid [x, y];
		}
	}

	public Node NodeFromCoordinates(int x, int y)
	{
		//Returns a Node from coordinates in integers
		return grid [x, y];
	}

	public Node NodeFromCoordinates(float x, float y)
	{
		//Returns a Node from coordinates in floats
		return grid [(int)x, (int)y];
	}

	public Vector3 CenterOfTwoNodes(Node n1, Node n2)
	{
		//Returns a Vector3 as the center of two nodes
		return new Vector3 (((n2.worldPosition.x + n1.worldPosition.x) / 2), 0, ((n2.worldPosition.z + n1.worldPosition.z) / 2));
	}

	public int DistanceBetweenTwoNodesX(Node n1, Node n2) 
	{
		//Returns an integer as the distance between two nodes' X values
		return (int)Mathf.Abs (n2.worldPosition.x - n1.worldPosition.x);
	}
	
	public int DistanceBetweenTwoNodesY(Node n1, Node n2)
	{
		//Returns an integer as the distance between two nodes' Y values
		return (int)Mathf.Abs (n2.worldPosition.z - n1.worldPosition.z);
	}
	
	public Vector2 NodeDifference(Node start, Node end)
	{
		//Returns a Vector2 storing the difference between two node's X and Y positions in its x and y values
		return new Vector2 (end.worldPosition.x - start.worldPosition.x, end.worldPosition.z - start.worldPosition.z);
			
	}

	public Node[] GetNeighbors(Node n)
	{
		Node[] neighbors = new Node[4];
		bool[] isEdge = new bool[4];

		int x = n.gridX;
		int y = n.gridY;

		if (x == 1) {
			isEdge [3] = true;
		} else if (x == gridSizeX - 1) {
			isEdge [1] = true;
		}

		if (y == 1) {
			isEdge [2] = true;
		} else if (y == gridSizeY - 1) {
			isEdge [0] = true;
		}

		if (isEdge [0] == false) {
			neighbors [0] = grid [x, y + 1];
		} else {
			neighbors [0] = null;
		}

		if (isEdge [1] == false) {
			neighbors [1] = grid [x + 1, y];
		} else {
			neighbors [1] = null;
		}

		if (isEdge [2] == false) {
			neighbors [2] = grid [x, y - 1];
		} else {
			neighbors [2] = null;
		}

		if (isEdge [3] == false) {
			neighbors [3] = grid [x - 1, y];
		} else {
			neighbors [3] = null;
		}
			
		return neighbors;
	}

	public bool NodeLineContainsWall(Node start, Node end, string xory) 
	{
		//Checks if the nodes between start and end nodes contain a wall. (Start and end must be on the same X or Y plane)
		int rangeStart = 0;
		int rangeEnd = 0;

		int otherAxis;

		if (xory == "x") {
			rangeStart = Mathf.Min (start.gridX, end.gridX);
			rangeEnd =  Mathf.Max (start.gridX, end.gridX);
			otherAxis =  start.gridY;

			for (int i = rangeStart; i <= rangeEnd; i++) {
				if (grid [i, otherAxis].Wall != null) {
					return true;
				}
			}
		} else {
			rangeStart = Mathf.Min (start.gridY, end.gridY);
			rangeEnd =  Mathf.Max (start.gridY, end.gridY);
			otherAxis = start.gridX;

			for (int i = rangeStart; i <= rangeEnd; i++) {
				if (grid [otherAxis, i].Wall != null) {
					return true;
				}
			}
		}

		return false;
	}

	/*public void setNodesAlongAxis(Node start, Node end, string xory, bool hasWall)
	{
		//Sets the nodes between start and end nodes to have walls or not. (Start and end must be on the same X or Y plane)
		int rangeStart = 0;
		int rangeEnd = 0;

		int otherAxis;

		if (xory == "x") {
			rangeStart = Mathf.Min (start.gridX, end.gridX);
			rangeEnd =  Mathf.Max (start.gridX, end.gridX);
			otherAxis =  start.gridY;

			for (int i = rangeStart; i <= rangeEnd; i++) {
				grid [i, otherAxis].hasWall = hasWall;
				Debug.Log ("X == " + i + " " + otherAxis + " SET!");
			}
		} else {
			rangeStart = Mathf.Min (start.gridY, end.gridY);
			rangeEnd =  Mathf.Max (start.gridY, end.gridY);
			otherAxis = start.gridX;

			for (int i = rangeStart; i <= rangeEnd; i++) {
				grid [otherAxis, i].hasWall = hasWall;
				Debug.Log ("Y == " + otherAxis + " " + i + " SET!");
			}
		}

		Debug.Log ("LINE NODES SET");
	}*/

}
