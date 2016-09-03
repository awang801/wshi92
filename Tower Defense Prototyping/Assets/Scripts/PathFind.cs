using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathFind:MonoBehaviour
{

    Grid grid;
    float[,] visited;
    Node current;
    Node destination;
    Node[] neighbors;
    Stack<Node> path;
    int stepsTaken;
	public Vector2 gridWorldSize;


    public bool pathFound(Node start, Node end)
    {
        grid = GetComponent<Grid>();
        destination = end;
        current = start;
		visited = new float[25, 35];
        for (int x = 0; x < 25; x++)
        {
            for (int y = 0; y < 35; y++)
            {
                visited[x, y] = 0;
            }
        }
        stepsTaken = 1;

        path = new Stack<Node>();
	    while (current.gridY != destination.gridY && stepsTaken != 0)
        {

            neighbors = grid.GetNeighbors(current);
            if (neighbors[2] != null && neighbors[2].Wall == null  && visited[neighbors[2].gridX, neighbors[2].gridY] == 0)
            {
				path.Push(neighbors[2]);
                current = neighbors[2];
                visited[neighbors[2].gridX, neighbors[2].gridY] = 1;
                stepsTaken++;
            }
            else if (neighbors[1] != null && neighbors[1].Wall == null && visited[neighbors[1].gridX, neighbors[1].gridY] == 0)
            {
				path.Push(neighbors[1]);
                current = neighbors[1];
                visited[neighbors[1].gridX, neighbors[1].gridY] = 1;
                stepsTaken++;
            }
            else if (neighbors[3] != null && neighbors[3].Wall == null && visited[neighbors[3].gridX, neighbors[3].gridY] == 0)
            {
				path.Push(neighbors[3]);
                current = neighbors[3];
                visited[neighbors[3].gridX, neighbors[3].gridY] = 1;
                stepsTaken++;
            }
            else if (neighbors[0] != null && neighbors[0].Wall == null && visited[neighbors[0].gridX, neighbors[0].gridY] == 0)
            {
				path.Push(neighbors[0]);
                current = neighbors[0];
                visited[neighbors[0].gridX, neighbors[0].gridY] = 1;
                stepsTaken++;
            }
			else
            {

				stepsTaken--;
				if(path.Count > 0)
				{
					path.Pop();
				}
				if(path.Count > 1)
				{
					current = path.Peek();
				}
            }
		}
        if (current.gridY == destination.gridY)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
