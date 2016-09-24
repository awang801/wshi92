using UnityEngine;
using System.Collections;

public class Node{
	//Used to create a grid representing each selectable point in the terrain

	public GameObject Wall;

	public GameObject Tower;
	public Vector3 worldPosition;
	public int gridX, gridY;

	public Node(Vector3 _worldPos, int _gridX, int _gridY)
	{
		worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
	}
		
}
