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
        // Create a new instance of the GridMap using the public parameters
        GridMap grid = new GridMap(rows, columns, tileListScriptable, cellSize);
    }

    // Update is called once per frame (if needed for further interactions)
    void Update()
    {

    }
}
