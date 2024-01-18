using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Array2DEditor;

public class Board : MonoBehaviour
{
    public bool boardFlowDirection = false;

    RaycastHit2D raycast;

    private const int height = 7;
    private const int width = 7;

    [SerializeField]
    private Array2DBool visualBoard = null;

    private Transform[,] nodes = new Transform[width, height];

    private void OnDrawGizmos()
    {
        DrawBoundaries();
        FillArray();
    }

    void DrawBoundaries()
    {
        Grid grid = GetComponent<Grid>();

        Vector2 bottomLeftCorner = new Vector2(-grid.cellSize.x / 2, -grid.cellSize.y / 2);

        Vector2 bottomRightCorner = new Vector2((grid.cellSize.x * visualBoard.GridSize.x) - grid.cellSize.x / 2, -grid.cellSize.y / 2);

        Vector2 topLeftCorner = new Vector2(-grid.cellSize.x / 2, (grid.cellSize.y * visualBoard.GridSize.y) - grid.cellSize.y / 2);

        Vector2 topRightCorner = new Vector2((grid.cellSize.x * visualBoard.GridSize.x) - grid.cellSize.x / 2, (grid.cellSize.y * visualBoard.GridSize.y) - grid.cellSize.y / 2);

        Debug.DrawLine(bottomLeftCorner, bottomRightCorner, Color.red);

        Debug.DrawLine(bottomRightCorner, topRightCorner, Color.red);

        Debug.DrawLine(topRightCorner, topLeftCorner, Color.red);

        Debug.DrawLine(topLeftCorner, bottomLeftCorner, Color.red);
    }

    void FillArray()
    {
        Grid grid = GetComponent<Grid>();

        Vector2 rayTarget = Vector2.zero;


        /*
         * VIUSAL GRID SIZE MUST BE CHANGED IN INSPECTOR!
         */


        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                raycast = Physics2D.Raycast(rayTarget, Vector3.forward);

                if (raycast.collider != null)
                {
                    visualBoard.SetCell(x, y, true);
                    nodes[x, y] = raycast.collider.transform;
                }
                else
                {
                    visualBoard.SetCell(x, y, false);
                    nodes[x, y] = null;
                }

                rayTarget = new Vector2(rayTarget.x + grid.cellSize.x, rayTarget.y);
            }
            rayTarget = new Vector2(0, rayTarget.y + grid.cellSize.y);
        }
    }


    private void Start()
    {
        //FillArray();
    }

    public Transform GetNodePos(Vector2Int index)
    {
        if ((index.x >= 0 && index.x < width) && (index.y >= 0 && index.y < height))
        {
            return nodes[index.x, index.y];
        } 
        else
        {
            //print("Outside Boundaries");
            return null;
        } 
    }

    public Vector2Int GetNodeIndex(Transform node)
    {
        Grid grid = GetComponent<Grid>();

        Vector2 rayTarget = Vector2.zero;

        Vector2Int index = Vector2Int.zero;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                raycast = Physics2D.Raycast(rayTarget, Vector3.forward);

                if (raycast.collider != null)
                {
                    if (raycast.collider.gameObject == node.gameObject)
                    {
                        index = new Vector2Int(x, y);
                        break;
                    }
                }

                rayTarget = new Vector2(rayTarget.x + grid.cellSize.x, rayTarget.y);
            }
            rayTarget = new Vector2(0, rayTarget.y + grid.cellSize.y);
        }

        return index;
    }
}
