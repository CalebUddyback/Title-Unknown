using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public bool boardFlowDirection = false;

    RaycastHit2D raycast;

    [SerializeField]
    private int height = 100;
    [SerializeField]
    private int width = 100;

    private Transform[,] tiles;

    const float TILE_SIZE = 1;

    public Transform startingTile;

    void DrawBoundaries()
    {
        Vector2 bottomLeftCorner = new Vector2(-TILE_SIZE / 2, -TILE_SIZE / 2);

        Vector2 bottomRightCorner = new Vector2((TILE_SIZE * width) - TILE_SIZE / 2, -TILE_SIZE / 2);

        Vector2 topLeftCorner = new Vector2(-TILE_SIZE / 2, (TILE_SIZE * height) - TILE_SIZE / 2);

        Vector2 topRightCorner = new Vector2((TILE_SIZE * width) - TILE_SIZE / 2, (TILE_SIZE * height) - TILE_SIZE / 2);

        Debug.DrawLine(bottomLeftCorner, bottomRightCorner, Color.red);

        Debug.DrawLine(bottomRightCorner, topRightCorner, Color.red);

        Debug.DrawLine(topRightCorner, topLeftCorner, Color.red);

        Debug.DrawLine(topLeftCorner, bottomLeftCorner, Color.red);
    }

    private void OnDrawGizmos()
    {
        DrawBoundaries();
    }

    void FillArray()
    {
        tiles = new Transform[width, height];

        Vector2 rayTarget = Vector2.zero;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                raycast = Physics2D.Raycast(rayTarget, Vector3.forward);

                if (raycast.collider != null)
                {
                    tiles[x, y] = raycast.collider.transform;
                }
                else
                {
                    tiles[x, y] = null;
                }

                rayTarget = new Vector2(rayTarget.x + TILE_SIZE, rayTarget.y);
            }
            rayTarget = new Vector2(0, rayTarget.y + TILE_SIZE);
        }
    }


    private void Awake()
    {
        FillArray();
    }

    private string DrawBoardMap()
    {
        string map = "\n";

        for (int y = height - 1; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                if (tiles[x, y] != null)
                    map += "* ";
                else
                    map += "- ";
            }
            map += "\n";
        }

        return map;
    }

    private void Start()
    {
        print(DrawBoardMap());
    }

    //public Tile GetTile(Vector2Int index)
    //{
    //    return tiles[index.x, index.y].GetComponent<Tile>();
    //}

    public Transform GetTilePos(Vector2Int index)
    {
        Transform pos = null;

        if ((index.x >= 0 && index.x < width) && (index.y >= 0 && index.y < height))
        {
            pos = tiles[index.x, index.y];
            goto Found;
        }

    Found:
        return pos;
    }

    public Vector2Int GetTileIndex(Transform tile)
    {
        Vector2Int index = Vector2Int.zero;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (tiles[x, y] == tile)
                {
                    index = new Vector2Int(x, y);
                    goto Found;
                }
            }
        }

    Found:
        return index;
    }
}
