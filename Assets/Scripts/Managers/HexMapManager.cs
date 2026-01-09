using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HexMapManager : MonoBehaviour
{
    #region Singleton

    public static HexMapManager Instance { get; private set; }

    #endregion

    #region Serialized Fields

    [Header("Hex Map Configuration")]
    [Tooltip("Number of columns in the hex grid")]
    public int columns = 10;

    [Tooltip("Number of rows in the hex grid")]
    public int rows = 10;

    [Tooltip("Size of each hex cell")]
    public float cellSize = 1f;


    #endregion

    #region Private Fields

    private DatabaseManager databaseManager;
    private TileFactory tileFactory;
    private bool configurationValid = false;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        databaseManager = DatabaseManager.Instance;
        tileFactory = TileFactory.Instance;

        if (databaseManager == null)
        {
            Debug.LogError("HexMapManager: DatabaseManager not found!");
            return;
        }

        if (tileFactory == null)
        {
            Debug.LogError("HexMapManager: TileFactory not found!");
            return;
        }

        databaseManager.initMapData.columns = columns;
        databaseManager.initMapData.rows = rows;
        databaseManager.initMapData.cellSize = cellSize;

        //GenerateMap();
    }

    #endregion

    #region Map Generation

    public void GenerateMap()
    {
        Debug.Log($"HexMapManager: Starting map generation ({columns}x{rows})...");

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                GenerateTileAt(col, row);
            }
        }

        SetupTileAdjacency();

        Debug.Log($"HexMapManager: Map generated with {databaseManager.TileDictionary.Count} tiles.");
    }

    private void GenerateTileAt(int col, int row)
    {
        Vector2Int dbPosition = new Vector2Int(col, row);
        Vector3 worldPosition = CalculateWorldPosition(col, row);

        

        KeyValuePair<ObjectIdentifier, DBTileValue> tileData =
            tileFactory.AddRandomTile(dbPosition, worldPosition, transform);

        if (tileData.Key == null || tileData.Value == null)
        {
            Debug.LogError($"HexMapManager: TileFactory failed to create tile at ({col}, {row})");
            return;
        }

        databaseManager.AddTile(tileData.Key, tileData.Value);
    }

    private void SetupTileAdjacency()
    {
        foreach (var kvp in databaseManager.TileDictionary)
        {
            DBTileValue tile = kvp.Value;
            tile.AdjacentTilesPosition = CalculateAdjacentPositions(tile.Position);
        }
    }

    #endregion

    #region Position Calculation

    private Vector3 CalculateWorldPosition(int column, int row)
    {
        float size = cellSize;
        bool shouldOffset = (row % 2) == 0;
        float width = Mathf.Sqrt(3) * size;
        float height = 2f * size;
        float horizontalDistance = width;
        float verticalDistance = height * (3f / 4f);
        float offset = shouldOffset ? width / 2 : 0;
        float xPosition = (column * horizontalDistance) + offset;
        float yPosition = row * verticalDistance;

        return new Vector3(xPosition, 0, -yPosition);
    }

    private List<Vector2Int> CalculateAdjacentPositions(Vector2Int position)
    {
        int col = position.x;
        int row = position.y;
        bool isEvenRow = (row % 2 == 0);

        List<Vector2Int> neighbors = new List<Vector2Int>();

        int[][] evenRowOffsets = new int[][]
        {
            new int[] { -1, 0 },  // Left
            new int[] { -1, -1 }, // Top-Left
            new int[] { 0, -1 },  // Top-Right
            new int[] { 1, 0 },   // Right
            new int[] { 0, 1 },   // Bottom-Right
            new int[] { -1, 1 }   // Bottom-Left
        };

        int[][] oddRowOffsets = new int[][]
        {
            new int[] { -1, 0 },  // Left
            new int[] { 0, -1 },  // Top-Left
            new int[] { 1, -1 },  // Top-Right
            new int[] { 1, 0 },   // Right
            new int[] { 1, 1 },   // Bottom-Right
            new int[] { 0, 1 }    // Bottom-Left
        };

        int[][] offsets = isEvenRow ? evenRowOffsets : oddRowOffsets;

        foreach (var offset in offsets)
        {
            int neighborCol = col + offset[0];
            int neighborRow = row + offset[1];

            if (neighborCol >= 0 && neighborCol < columns &&
                neighborRow >= 0 && neighborRow < rows)
            {
                neighbors.Add(new Vector2Int(neighborCol, neighborRow));
            }
        }

        return neighbors;
    }

    #endregion

    #region Database Wrapper Methods

    public DBTileValue GetTileValue(ObjectIdentifier tileId)
    {
        return DatabaseManager.Instance.GetTileValue(tileId);
    }

    public List<DBTileValue> GetTileNeighbors(ObjectIdentifier tileId)
    {
        return DatabaseManager.Instance.GetTileNeighbors(tileId);
    }

    public ObjectIdentifier GetTileIdByObject(GameObject tileObject)
    {
        return DatabaseManager.Instance.GetTileIdByObject(tileObject);
    }

    public ResourceType GetTileResourceType(ObjectIdentifier tileId)
    {
        return DatabaseManager.Instance.GetTileResourceType(tileId);
    }

    public GameObject GetTileGameObject(ObjectIdentifier tileId)
    {
        return DatabaseManager.Instance.GetTileGameObject(tileId);
    }

    #endregion
}
