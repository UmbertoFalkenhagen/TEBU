using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class HexMapGenerator : MonoBehaviour
{
    // string seed;
    private DatabaseManager databaseManager;
    GridPosition position;

    private void Awake()
    {
        //Attach Database
        if (databaseManager == null)
        {
            databaseManager = FindObjectOfType<DatabaseManager>();
            if (databaseManager == null)
            {
                Debug.LogError("DatabaseManager could not be found!");
            }
        }
    }
    public void GenerateMap()
    {
        // Check if databaseManager is null or empty before proceeding
        if (DatabaseManager.Instance != null) {
            // loop Cols and Rows
            for (int i = 0;i< databaseManager.initMapData.columns; i++)
            {
                for (int j = 0;j< databaseManager.initMapData.rows; j++)
                {
                    position = new(i, j);
                    //define Tile Grid -> find in old script
                    // tileFactory.build(getPositionForTile(i, j));
                    //TODO: ....Factory Tile ... Replace following two lines with factory call
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = getPositionForTile(i, j);
                    //Add Tile to Dictionary
                    databaseManager.AddTile(position);
                    checkTileDictionary();
                }
            }
            //TODO: register adjecent tiles
            getAdjacentTiles();
        }
        else
        {
            Debug.LogError("DatabaseManager.Instance ist null!");
        }
    }

    private void checkTileDictionary()
    {
        if (DatabaseManager.Instance.tileDictionary.Count > 0)
        {
            Debug.Log("tileDictionary enthält " + DatabaseManager.Instance.tileDictionary.Count + " Einträge.");
        }
        else
        {
            Debug.Log("tileDictionary ist leer.");
        }

    }

    private Vector3 getPositionForTile(int _column, int _row)
    {
        //some Math to get World Positions from cells and rows
        int column = _column;
        int row = _row;
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

        //TODO: Fix that out of bounce tiles are added too
        List<GridPosition> neighbors = new List<GridPosition>();

        foreach (var value in databaseManager.tileDictionary.Values)
        {
            int row = value.Position.row;
            int column = value.Position.column;

            bool isEvenRow = (row % 2) == 0;

            if (isEvenRow)
            {
                neighbors.Add(new GridPosition(column - 1, row));     // Links
                neighbors.Add(new GridPosition(column + 1, row));     // Rechts
                neighbors.Add(new GridPosition(column, row - 1));     // Oben
                neighbors.Add(new GridPosition(column, row + 1));     // Unten
                neighbors.Add(new GridPosition(column - 1, row + 1)); // Links unten
                neighbors.Add(new GridPosition(column - 1, row - 1)); // Links oben
            }
            else
            {
                neighbors.Add(new GridPosition(column - 1, row));     // Links
                neighbors.Add(new GridPosition(column + 1, row));     // Rechts
                neighbors.Add(new GridPosition(column, row - 1));     // Oben
                neighbors.Add(new GridPosition(column, row + 1));     // Unten
                neighbors.Add(new GridPosition(column + 1, row + 1)); // Rechts unten
                neighbors.Add(new GridPosition(column + 1, row - 1)); // Rechts oben
            }
            value.AdjacentTilesPosition = neighbors;
           // Debug.Log(neighbors[1].row.ToString() + ", " + neighbors[1].column.ToString());
        }


    }
}
