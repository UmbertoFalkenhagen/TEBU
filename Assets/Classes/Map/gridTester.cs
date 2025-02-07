using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gridTester : MonoBehaviour
{
    public int rows;                         // Number of rows in the hex grid
    public int columns;                      // Number of columns in the hex grid
    public float cellSize;                   // Size of each hex cell
    public List<ScriptableTile> tileListScriptable;  // List of ScriptableTile objects
    public List<ScriptableCityCenter> cityCenterScriptables;
    public List<ScriptableBuilding> buildingScriptables;

    private GridMap grid;  // Reference to the GridMap component

    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid();  // Generate the hex grid on startup

        //Invoke(nameof(TestPathfinding), 1f);  // Call TestPathfinding with a slight delay to ensure grid is initialized
        //Invoke(nameof(TestFindTilesAtEdgeDistance), 1f);  // Call TestFindTilesAtEdgeDistance after grid initialization
        Invoke(nameof(TestCityCenterPlacement), 1f);  // Call TestCityCenterPlacement with a delay to ensure grid is initialized
        SoundManager.Instance.PlaySoundForever("Main");
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
            grid.InitializeGrid(rows, columns, tileListScriptable, cityCenterScriptables, buildingScriptables, cellSize);
        }
        else
        {
            Debug.Log("GridMap already initialized. Skipping duplicate initialization.");
        }
    }

    // Test the pathfinding between two randomly selected tiles from the GridMap's dictionary
    void TestPathfinding()
    {
        if (grid == null || grid.tileDictionary == null || grid.tileDictionary.Count < 2)
        {
            Debug.LogError("GridMap is not fully initialized or tileDictionary has fewer than 2 tiles.");
            return;
        }

        // Get two distinct random tiles and their indexes from the grid
        (GameObject startTile, (int, int) startIndex) = grid.GetRandomTile();
        (GameObject endTile, (int, int) endIndex) = grid.GetRandomTile();

        while (endTile == startTile) // Ensure start and end tiles are different
        {
            (endTile, endIndex) = grid.GetRandomTile();
        }

        Debug.Log($"Pathfinding Test: Start Tile at {startIndex}, End Tile at {endIndex}");

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

    void TestFindTilesAtEdgeDistance()
    {
        if (grid == null || grid.tileDictionary == null || grid.tileDictionary.Count == 0)
        {
            Debug.LogError("GridMap is not initialized or tileDictionary is empty.");
            return;
        }

        // Select a random tile from the grid and get its index
        (GameObject randomTile, (int, int) randomTileIndex) = grid.GetRandomTile();
        if (randomTile == null)
        {
            Debug.LogError("Failed to retrieve a random tile.");
            return;
        }

        float range = 3f;

        // Find tiles exactly at the edge distance of 3f from the selected tile
        List<GameObject> edgeTiles = grid.FindTilesAtEdgeDistance(randomTile, range);

        // Log the results
        Debug.Log("Number of tiles at edge distance: " + edgeTiles.Count);
        Debug.Log($"Tiles exactly at distance {range} from tile at index {randomTileIndex}:");
        foreach (GameObject tile in edgeTiles)
        {
            HexTile hexTile = tile.GetComponent<HexTile>();
            if (hexTile != null)
            {
                Debug.Log($"Tile at index: {hexTile.HexCoords}");
            }
        }

        // Optional: Visualize the found edge tiles as a path
        if (edgeTiles.Count > 0)
        {
            Pathfinder.Instance.VisualizePath(edgeTiles);
        }
    }

    // Test function to place a city center on a randomly selected tile
    void TestCityCenterPlacement()
    {
        // Get a random tile from the grid
        (GameObject randomTile, (int, int) tileIndex) = grid.GetRandomTile();
        if (randomTile == null)
        {
            Debug.LogError("Failed to retrieve a random tile.");
            return;
        }

        // Get the HexTile component to check the tile's type
        HexTile hexTile = randomTile.GetComponent<HexTile>();
        if (hexTile == null)
        {
            Debug.LogError("HexTile component missing on the selected random tile.");
            return;
        }

        TileType tileType = hexTile.TileType;

        // Find a ScriptableCityCenter that can be constructed on this tile type
        ScriptableCityCenter matchingCityCenter = cityCenterScriptables.Find(center => center.cityLocation == tileType);

        if (matchingCityCenter != null)
        {
            Debug.Log($"Placing city center of type '{matchingCityCenter.name}' on tile at index {tileIndex} of type '{tileType}'.");

            // Use the CityCenterFactory to create the city center on this tile
            GameObject cityCenterObject = CityCenterFactory.Instance.CreateObject(matchingCityCenter, randomTile, Quaternion.identity, randomTile);
            if (cityCenterObject != null)
            {
                hexTile.heldBuilding = cityCenterObject;  // Store the city center in the tile's held building
                hexTile.ClearTileResource();  // Deactivate any existing resource on the tile
            }
            else
            {
                Debug.LogError("Failed to instantiate city center on the selected tile.");
            }
        }
        else
        {
            Debug.LogWarning($"No matching city center found for tile of type '{tileType}' at index {tileIndex}.");
        }
    }


    // Update is called once per frame (if needed for further interactions)
    void Update()
    {

    }
}
