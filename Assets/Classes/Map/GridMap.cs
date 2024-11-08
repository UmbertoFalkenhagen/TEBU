using UnityEngine;
using System.Collections.Generic;

public class GridMap : MonoBehaviour
{
    #region Singleton
    public static GridMap Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Destroy any duplicate GridMap instances
            return; // Prevents further initialization in this instance
        }
    }
    #endregion

    public int rows;
    public int columns;
    public float cellSize;
    public Dictionary<(int, int), GameObject> tileDictionary = new Dictionary<(int, int), GameObject>();
    public List<ScriptableTile> tileListScriptable;

    private void Start()
    {
        InitializePathfinder(); // Ensure Pathfinder exists
        if (tileDictionary.Count == 0) // Only initialize the grid if it hasn't been populated yet
        {
            InitializeGrid(rows, columns, tileListScriptable, cellSize);
        }
    }

    private void InitializePathfinder()
    {
        if (Pathfinder.Instance == null)
        {
            gameObject.AddComponent<Pathfinder>();
        }
    }

    public void InitializeGrid(int _rows, int _columns, List<ScriptableTile> _tileListScriptable, float _cellSize)
    {
        this.rows = _rows;
        this.columns = _columns;
        this.tileListScriptable = _tileListScriptable;
        this.cellSize = _cellSize;

        if (tileListScriptable == null || tileListScriptable.Count == 0)
        {
            Debug.LogError("Tile list is null or empty! Please assign a valid list of ScriptableTile assets.");
            return;
        }

        tileDictionary.Clear(); // Clear any existing entries in the dictionary

        // Initialize each tile and add it to the dictionary
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < columns; y++)
            {
                Vector3 hexPosition = GetHexWorldPosition(new Vector2Int(x, y));
                ScriptableTile chosenTile = ChooseRandomScriptableTile();

                if (chosenTile == null)
                {
                    Debug.LogError($"No valid tile for position ({x}, {y}).");
                    continue;
                }

                GameObject hexTileObject = TileFactory.Instance.CreateObject(chosenTile, hexPosition, Quaternion.identity, transform);
                if (hexTileObject == null)
                {
                    Debug.LogError($"Failed to create tile at position ({x}, {y}).");
                    continue;
                }

                hexTileObject.GetComponent<HexTile>().HexCoords = new Vector2Int(x, y);
                tileDictionary[(x, y)] = hexTileObject;
            }
        }

        // After all tiles are initialized, find neighbors for each tile
        foreach (var tile in tileDictionary.Values)
        {
            HexTile hexTileComponent = tile.GetComponent<HexTile>();
            if (hexTileComponent != null)
            {
                hexTileComponent.FindNeighbors(1.5f * cellSize); // Use a range to capture immediate neighbors
            }
        }
    }


    private Vector3 GetHexWorldPosition(Vector2Int coordinate)
    {
        int column = coordinate.x;
        int row = coordinate.y;

        float size = cellSize;
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

    private ScriptableTile ChooseRandomScriptableTile()
    {
        if (tileListScriptable == null || tileListScriptable.Count == 0)
        {
            Debug.LogError("No available tiles to choose from in tileListScriptable.");
            return null;
        }

        return tileListScriptable[Random.Range(0, tileListScriptable.Count)];
    }

    public GameObject GetTileByIndex(int x, int y)
    {
        tileDictionary.TryGetValue((x, y), out GameObject tile);
        return tile;
    }

    // Get tile by world position
    public GameObject GetTileByCoordinates(Vector3 worldPosition)
    {
        foreach (var tile in tileDictionary)
        {
            if (Vector3.Distance(tile.Value.transform.position, worldPosition) < cellSize / 2)
            {
                return tile.Value;
            }
        }
        return null;
    }

    // Get index by tile GameObject
    public (int, int)? GetIndexByTile(GameObject tile)
    {
        foreach (var kvp in tileDictionary)
        {
            if (kvp.Value == tile)
            {
                return kvp.Key;
            }
        }
        return null;
    }

    public List<GameObject> FindTilesAtEdgeDistance(GameObject originTile, float range)
    {
        List<GameObject> edgeTiles = new List<GameObject>();
        Vector3 originPosition = originTile.transform.position;

        Collider[] colliderArray = Physics.OverlapSphere(originPosition, cellSize * range *1.5f);
        foreach (Collider collider3D in colliderArray)
        {
            HexTile tile = collider3D.GetComponentInParent<HexTile>();
            edgeTiles.Add(tile.gameObject);
            
        }

        return edgeTiles;
    }

    //GetIndexByCoordinates
    //GetCoordinatesByTile
    //GetCoordinatesByIndex



}
