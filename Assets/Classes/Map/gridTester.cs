using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gridTester : MonoBehaviour
{
    public int rows;                         // Number of rows in the hex grid
    public int columns;                      // Number of columns in the hex grid
    public float cellSize;                   // Size of each hex cell
    public List<ScriptableTile> tileListScriptable;  // List of ScriptableTile objects

    private GridMap grid;  // Reference to the GridMap component

    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid();  // Generate the hex grid on startup
       // Invoke(nameof(TestPathfinding), 1f);  // Call TestPathfinding with a slight delay to ensure grid is initialized
    }

    // Generates the grid using the GridMap class
    void GenerateGrid()
    {
        // Check if tileListScriptable is null or empty before proceeding
        if (tileListScriptable == null || tileListScriptable.Count == 0)
        {
            Debug.LogError("Tile list is not assigned or empty in gridTester. Please assign ScriptableTile assets in the inspector.");
            return;
        }

        // Check if GridMap already exists
        if (GridMap.Instance == null)
        {
            // Create a new GameObject to attach the GridMap component to
            GameObject gridMapObject = new GameObject("GridMap");

            // Add the GridMap component to the newly created GameObject
            grid = gridMapObject.AddComponent<GridMap>();
        }
        else
        {
            // Use the existing GridMap instance
            grid = GridMap.Instance;
        }

        // Only initialize the grid if it hasn't been initialized already
        if (grid.tileDictionary == null || grid.tileDictionary.Count == 0)
        {
            grid.InitializeGrid(rows, columns, tileListScriptable, cellSize);
        }
        else
        {
            Debug.Log("GridMap already initialized. Skipping duplicate initialization.");
        }
    }

    // Test the pathfinding between two randomly selected tiles from the GridMap's dictionary
    void TestPathfinding()
    {
        // Ensure the grid and tileDictionary are initialized
        if (grid == null || grid.tileDictionary == null || grid.tileDictionary.Count < 2)
        {
            Debug.LogError("GridMap is not fully initialized or tileDictionary has fewer than 2 tiles.");
            return;
        }

        // Convert the tile dictionary keys (coordinates) to a list
        List<(int, int)> tileCoordinates = new List<(int, int)>(grid.tileDictionary.Keys);

        // Randomly select two tiles from the list
        int startTileIndex = Random.Range(0, tileCoordinates.Count);
        int endTileIndex = Random.Range(0, tileCoordinates.Count);

        // Ensure that start and end tiles are different
        while (startTileIndex == endTileIndex)
        {
            endTileIndex = Random.Range(0, tileCoordinates.Count);
        }

        // Get the corresponding GameObjects for the randomly selected start and end tiles
        (int, int) startCoordinates = tileCoordinates[startTileIndex];
        (int, int) endCoordinates = tileCoordinates[endTileIndex];

        GameObject startTile = grid.GetTileByIndex(startCoordinates.Item1, startCoordinates.Item2);
        GameObject endTile = grid.GetTileByIndex(endCoordinates.Item1, endCoordinates.Item2);

        if (startTile == null || endTile == null)
        {
            Debug.LogError("Start or End tile not found. Pathfinding test aborted.");
            return;
        }

        Debug.Log($"Pathfinding Test: Start Tile at {startCoordinates}, End Tile at {endCoordinates}");

        // Calculate the path between the start and end tiles
        List<GameObject> path = Pathfinder.Instance.CalculatePath(startTile, endTile);

        // Log the path to the console and visualize it if a path was found
        if (path != null && path.Count > 0)
        {
            Debug.Log("Path found:");
            foreach (GameObject tile in path)
            {
                HexTile hexTile = tile.GetComponent<HexTile>();
                if (hexTile != null)
                {
                    Debug.Log($"Tile at: {hexTile.HexCoords}");  // Log each tile’s HexCoords in the path
                }
            }

            // Visualize the path on the screen
            Pathfinder.Instance.VisualizePath(path);
        }
        else
        {
            Debug.LogWarning("No path found between the selected tiles.");
        }
    }

    // Update is called once per frame (if needed for further interactions)
    void Update()
    {

    }
}
