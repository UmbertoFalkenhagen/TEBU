using UnityEngine;
using System.Collections.Generic;

public class GridMap : MonoBehaviour
{
    private int rows;
    private int columns;
    private float cellSize;
    private int[,] gridArray;
    private List<ScriptableTile> tileListScriptable;

    // Initialization method to set up the grid map
    public void InitializeGrid(int _rows, int _columns, List<ScriptableTile> _tileListScriptable, float _cellSize)
    {
        this.rows = _rows;
        this.columns = _columns;
        this.tileListScriptable = _tileListScriptable;
        this.cellSize = _cellSize;

        // Check if tileListScriptable is properly assigned
        if (tileListScriptable == null || tileListScriptable.Count == 0)
        {
            Debug.LogError("Tile list is null or empty! Please assign a valid list of ScriptableTile assets.");
            return;
        }

        gridArray = new int[rows, columns];

        // Loop through each coordinate to create the grid
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                // Get the hex position for the current coordinates
                Vector3 hexPosition = GetHexWorldPostition(new Vector2Int(x, y));

                // Choose a random ScriptableTile for this position
                ScriptableTile chosenTile = ChooseRandomTile();

                // Check if the chosen tile is valid
                if (chosenTile == null)
                {
                    Debug.LogError($"Chosen tile at position ({x}, {y}) is null!");
                    continue;
                }

                // Instantiate the tile using the TileFactory
                GameObject hexTileObject = TileFactory.Instance.CreateElement(chosenTile, hexPosition, Quaternion.identity, transform);
                if (hexTileObject == null)
                {
                    Debug.LogError($"Failed to create tile at position ({x}, {y}).");
                    continue;
                }

                // Get the initial object to place on the tile using the TileFactory
                GameObject initialObject = TileFactory.Instance.GetInitialObjectForTile(chosenTile, out Resource assignedResource); // Correct method call with out parameter

                // Store the assigned resource in the HexTile component
                hexTileObject.GetComponent<HexTile>().resource = assignedResource;

                // Place the initial object on the tile
                if (initialObject != null)
                {
                    hexTileObject.GetComponent<HexTile>().PlaceResourceOnTile(initialObject);
                }
            }
        }
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

    // Choose a random ScriptableTile from the available list
    private ScriptableTile ChooseRandomTile()
    {
        if (tileListScriptable == null || tileListScriptable.Count == 0)
        {
            Debug.LogError("No available tiles to choose from in tileListScriptable.");
            return null;
        }

        return tileListScriptable[Random.Range(0, tileListScriptable.Count)];
    }
}
