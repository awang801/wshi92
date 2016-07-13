using UnityEngine;
using System.Collections;

public class Node{

	public bool hasWall;
	public bool hasTower;
	public Vector3 worldPosition;
	public int gridX, gridY;

	public Node(bool _hasWall, bool _hasTower, Vector3 _worldPos, int _gridX, int _gridY)
	{
		hasWall = _hasWall;
		hasTower = _hasTower;
		worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
	}
		
}
