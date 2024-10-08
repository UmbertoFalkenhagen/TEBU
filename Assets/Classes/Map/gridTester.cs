using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gridTester : MonoBehaviour
{
    public int rows;                         // Number of rows in the hex grid
    public int columns;                      // Number of columns in the hex grid
    public float cellSize;                   // Size of each hex cell
    public List<ScriptableTile> tileListScriptable;  // List of ScriptableTile objects

    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid();  // Generate the hex grid on startup
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

        // Create a new GameObject to attach the GridMap component to
        GameObject gridMapObject = new GameObject("GridMap");

        // Add the GridMap component to the newly created GameObject
        GridMap grid = gridMapObject.AddComponent<GridMap>();

        // Set up the grid map by calling a separate initialization method (since constructors can't be used)
        grid.InitializeGrid(rows, columns, tileListScriptable, cellSize);
    }

    // Update is called once per frame (if needed for further interactions)
    void Update()
    {

    }
}
