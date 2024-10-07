using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap : MonoBehaviour
{
    private int rows;
    private int columns;
    private float cellSize;
    private int[,] gridArray;
    private List<ScriptableTile> tileListScriptable;  // Use ScriptableTile instead of ScriptableTiles

    // Constructor to initialize the grid map
    public GridMap(int _rows, int _columns, List<ScriptableTile> _tileListScriptable, float _cellSize)
    {
        this.rows = _rows;
        this.columns = _columns;
        this.tileListScriptable = _tileListScriptable;
        this.cellSize = _cellSize;

        gridArray = new int[rows, columns];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                // Get the hex position for the current coordinates
                Vector3 hexPosition = GetHexWorldPostition(new Vector2Int(x, y));
                Debug.Log(hexPosition);

                // Instantiate a random tile from the list at the calculated position
                GameObject hexTile = Instantiate(RandomTiles(), hexPosition, Quaternion.identity);
                hexTile.name = $"Hex_{x}_{y}";  // Set a unique name for the tile
            }
        }
    }

    // Calculate the world position for square grid coordinates
    private Vector3 GetWorldPostition(int x, int y)
    {
        return new Vector3(x, 0, y) * cellSize;
    }

    // Calculate the world position for hexagonal grid coordinates
    private Vector3 GetHexWorldPostition(Vector2Int coordinate)
    {
        int column = coordinate.x;
        int row = coordinate.y;

        float size = cellSize;   // Hexagon size (radius)
        bool shouldOffset = (row % 2) == 0;  // Determine if this row should be offset
        float width = Mathf.Sqrt(3) * size;  // Width of a hexagon
        float height = 2f * size;            // Height of a hexagon
        float horizontalDistance = width;    // Horizontal distance between hexagons
        float verticalDistance = height * (3f / 4f);  // Vertical distance between hexagons (3/4 height)
        float offset = (shouldOffset) ? width / 2 : 0; // Offset every second row for pointy-topped hexagons

        float xPosition = (column * horizontalDistance) + offset;  // X position of the hex
        float yPosition = row * verticalDistance;                  // Y position of the hex

        return new Vector3(xPosition, 0, -yPosition);  // Return the calculated position
    }

    // Choose a random tile prefab from the available scriptable tiles
    private GameObject RandomTiles()
    {
        if (tileListScriptable == null || tileListScriptable.Count == 0)
        {
            Debug.LogError("No tiles available in tileListScriptable.");
            return null;
        }

        // Choose a random ScriptableTile from the list
        ScriptableTile chosenTile = tileListScriptable[Random.Range(0, tileListScriptable.Count)];
        Debug.Log($"Chosen Tile: {chosenTile.tileType} with Prefab: {chosenTile.prefab.name}");

        // Return the prefab associated with this tile
        return chosenTile.prefab;
    }
}
