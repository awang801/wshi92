﻿using UnityEngine;
using System.Collections;

public class Grid : MonoBehaviour {

	public Vector2 gridWorldSize;
	public float nodeRadius;
	public LayerMask terrainMask;
	Node[,] grid;
	float nodeDiameter;
	int gridSizeX, gridSizeY;

	void Start()
	{
		nodeDiameter = nodeRadius * 2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

		CreateGrid ();
	}

	void CreateGrid()
	{
		grid = new Node[gridSizeX,gridSizeY];

		for (int x = 0; x < gridSizeX; x++) {
			for (int y = 0; y < gridSizeY; y++) {
				grid[x,y] = new Node(false, false, new Vector3(x, 0, y), x, y);
			}
		}
	}

	void OnDrawGizmos()
	{
		Vector3 box = new Vector3 (gridWorldSize.x, 1, gridWorldSize.y);
		Gizmos.DrawWireCube (transform.position + box * 0.5f, box);
	}

	public Node NodeFromWorldPoint(Vector3 worldPosition)
	{
		int x = Mathf.CeilToInt (worldPosition.x);
		int y = Mathf.CeilToInt (worldPosition.z);

		if (x > gridSizeX && y > gridSizeY) {
			
			return grid [gridSizeX, gridSizeY];

		}
		else if (x > gridSizeX) {
			
			return grid [gridSizeX, y];

		} else if (y > gridSizeY) {
			
			return grid [x, gridSizeY];

		}else {
			return grid [x, y];
		}
	}

	public Node NodeFromCoordinates(int x, int y)
	{
		return grid [x, y];
	}

	public Node NodeFromCoordinates(float x, float y)
	{
		return grid [(int)x, (int)y];
	}

	public Vector3 CenterOfTwoNodes(Node n1, Node n2)
	{
		return new Vector3 (((n2.worldPosition.x + n1.worldPosition.x) / 2), 0, ((n2.worldPosition.z + n1.worldPosition.z) / 2));
	}

	public float DistanceBetweenTwoNodesX(Node n1, Node n2)
	{
		return (int)Mathf.Abs (n2.worldPosition.x - n1.worldPosition.x);
	}
	
	public float DistanceBetweenTwoNodesY(Node n1, Node n2)
	{
		return (int)Mathf.Abs (n2.worldPosition.z - n1.worldPosition.z);
	}
	
	public Vector2 NodeDifference(Node start, Node end)
	{
		return new Vector2 (end.worldPosition.x - start.worldPosition.x, end.worldPosition.z - start.worldPosition.z);
			
	}

	public bool NodeLineContainsWall(Node start, Node end, string xory)
	{
		int rangeStart = 0;
		int rangeEnd = 0;

		int otherAxis;

		if (xory == "x") {
			rangeStart = Mathf.Min (start.gridX, end.gridX);
			rangeEnd =  Mathf.Max (start.gridX, end.gridX);
			otherAxis =  start.gridY;

			for (int i = rangeStart; i < rangeEnd; i++) {
				if (grid [i, otherAxis].hasWall == true) {
					return true;
				}
			}
		} else {
			rangeStart = Mathf.Min (start.gridY, end.gridY);
			rangeEnd =  Mathf.Max (start.gridY, end.gridY);
			otherAxis = start.gridX;

			for (int i = rangeStart; i < rangeEnd; i++) {
				if (grid [otherAxis, i].hasWall == true) {
					return true;
				}
			}
		}

		return false;
	}

	public void setNodesAlongAxis(Node start, Node end, string xory, bool hasWall)
	{
		int rangeStart = 0;
		int rangeEnd = 0;

		int otherAxis;

		if (xory == "x") {
			rangeStart = Mathf.Min (start.gridX, end.gridX);
			rangeEnd =  Mathf.Max (start.gridX, end.gridX);
			otherAxis =  start.gridY;

			for (int i = rangeStart; i < rangeEnd; i++) {
				grid [i, otherAxis].hasWall = hasWall;
				Debug.Log ("X == " + i + " " + otherAxis + " SET!");
			}
		} else {
			rangeStart = Mathf.Min (start.gridY, end.gridY);
			rangeEnd =  Mathf.Max (start.gridY, end.gridY);
			otherAxis = start.gridX;

			for (int i = rangeStart; i < rangeEnd; i++) {
				grid [otherAxis, i].hasWall = hasWall;
				Debug.Log ("Y == " + otherAxis + " " + i + " SET!");
			}
		}

		Debug.Log ("LINE NODES SET");
	}

}
