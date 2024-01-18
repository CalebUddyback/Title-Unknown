using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGrid
{
    private int width;
    private int height;
    private float nodeSize;
    private int[,] gridArray;

    public NodeGrid(int width, int height, float nodeSize)
    {
        this.width = width;
        this.height = height;
        this.nodeSize = nodeSize;

        gridArray = new int[width, height];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.yellow, 100f);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.yellow, 100f);
            }
        }
    }

    private Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * nodeSize;
    }
}
