using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMapManager : MonoBehaviour
{
    private DatabaseManager databaseManager;
    public static HexMapManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        databaseManager = DatabaseManager.Instance;
        if (databaseManager == null)
        {
            Debug.LogError("DatabaseManager could not be found!");
        }
    }

    public void GenerateMap()
    {
        if (DatabaseManager.Instance == null)
        {
            Debug.LogError("DatabaseManager.Instance is null!");
            return;
        }
        if (TileFactory.Instance == null)
        {
            Debug.LogError("TileFactory.Instance is null!");
            return;
        }

        // Example
        for (int i = 0; i < DatabaseManager.Instance.initMapData.columns; i++)
        {
            for (int j = 0; j < DatabaseManager.Instance.initMapData.rows; j++)
            {
                Vector2Int dbPosition = new Vector2Int(i, j);
                Vector3 worldPosition = GetPositionForTile(i, j);


                KeyValuePair<ObjectIdentifier, DBTileValue> tile = TileFactory.Instance.AddRandomTile(dbPosition, worldPosition);

                if (tile.Value != null)
                {
                    // e.g. random tile
                    DatabaseManager.Instance.AddTile(tile.Key, tile.Value);
                }
                // or tile of specific type:
                // TileFactory.Instance.AddTileOfType(TileType.Grassland, dbPosition, worldPosition);
            }
        }

        // adjacency, etc...
        checkTileDictionary();
        getAdjacentTiles();
    }

    private void checkTileDictionary()
    {
        if (databaseManager.TileDictionary.Count > 0)
        {
          //  Debug.Log("tileDictionary enthält " + databaseManager.TileDictionary.Count + " Einträge.");
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
        foreach (var value in databaseManager.TileDictionary.Values)
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
