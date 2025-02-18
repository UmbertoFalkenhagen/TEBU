using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMapGenerator : MonoBehaviour
{
    private DatabaseManager databaseManager;

    private void Awake()
    {
        databaseManager = DatabaseManager.Instance;
        if (databaseManager == null)
        {
            Debug.LogError("DatabaseManager could not be found!");
        }
    }

    public void GenerateMap()
    {
        if (databaseManager == null)
        {
            Debug.LogError("DatabaseManager.Instance is null!");
            return;
        }

        // Example loop
        for (int i = 0; i < databaseManager.initMapData.columns; i++)
        {
            for (int j = 0; j < databaseManager.initMapData.rows; j++)
            {
                // 1) DB position
                Vector2Int dbPosition = new Vector2Int(i, j);

                // 2) World position
                Vector3 worldPosition = GetPositionForTile(i, j);

                // 3) Add a random tile for demonstration
                databaseManager.AddRandomTile(dbPosition, worldPosition);

                // or if you want a specific tile type:
                // databaseManager.AddTileOfType(TileType.Grassland, dbPosition, worldPosition);
            }
        }

        checkTileDictionary();
        getAdjacentTiles();
    }

    private void checkTileDictionary()
    {
        if (databaseManager.tileDictionary.Count > 0)
        {
            Debug.Log("tileDictionary enthält " + databaseManager.tileDictionary.Count + " Einträge.");
        }
        else
        {
            Debug.Log("tileDictionary ist leer.");
        }
    }

    private Vector3 GetPositionForTile(int column, int row)
    {
        float size = databaseManager.initMapData.cellSize;
        bool shouldOffset = (row % 2) == 0;
        float width = Mathf.Sqrt(3) * size;
        float height = 2f * size;
        float horizontalDistance = width;
        float verticalDistance = height * (3f / 4f);
        float offset = (shouldOffset) ? width / 2 : 0;
        float xPosition = (column * horizontalDistance) + offset;
        float yPosition = row * verticalDistance;

        return new Vector3(xPosition, 0, -yPosition);
    }

    public void getAdjacentTiles()
    {
        // TODO: Fix out-of-bounds references
        foreach (var value in databaseManager.tileDictionary.Values)
        {
            int col = value.Position.x;
            int row = value.Position.y;
            bool isEvenRow = (row % 2) == 0;

            List<Vector2Int> neighbors = new List<Vector2Int>();
            if (isEvenRow)
            {
                neighbors.Add(new Vector2Int(col - 1, row));     // Left
                neighbors.Add(new Vector2Int(col + 1, row));     // Right
                neighbors.Add(new Vector2Int(col, row - 1));     // Up
                neighbors.Add(new Vector2Int(col, row + 1));     // Down
                neighbors.Add(new Vector2Int(col - 1, row + 1)); // Bottom-left
                neighbors.Add(new Vector2Int(col - 1, row - 1)); // Top-left
            }
            else
            {
                neighbors.Add(new Vector2Int(col - 1, row));
                neighbors.Add(new Vector2Int(col + 1, row));
                neighbors.Add(new Vector2Int(col, row - 1));
                neighbors.Add(new Vector2Int(col, row + 1));
                neighbors.Add(new Vector2Int(col + 1, row + 1));
                neighbors.Add(new Vector2Int(col + 1, row - 1));
            }

            // Assign the neighbor list to the DBTileValue
            value.AdjacentTilesPosition = neighbors;
        }
    }
}
