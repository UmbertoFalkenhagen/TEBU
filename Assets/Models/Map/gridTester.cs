using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gridTester : MonoBehaviour
{
    public List<ScriptableTiles> gridBlock;
    public int rows;
    public int columns;
    public float cellSize;
    public List<ScriptableTiles> tileListScriptable;

    private ScriptableTiles currentTile;
    // Start is called before the first frame update
    void Start()
    {
        generateGrid();
    }

    void generateGrid()
    {
        GridMap grid = new GridMap(rows, columns, tileListScriptable, cellSize);

    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
