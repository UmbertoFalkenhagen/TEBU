using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexMapGenerator : MonoBehaviour
{

    int rows;
    int cols;
    // string seed;
    private DatabaseManager databaseManager;

    // float cellSize;
    // Start is called before the first frame update
    void Start()
    {
    }
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

    // Update is called once per frame
    void Update()
    {

    }

    public void GenerateMap()
    {
        // Check if tileListScriptable is null or empty before proceeding
        if (DatabaseManager.Instance != null) {
            checkTileDictionary();

            // loop TileFactory... 

            //TODO: ....Factory Tile ...
            rows = databaseManager.initMapData.rows;
            cols = databaseManager.initMapData.columns;
            //TODO: Add Tile to Dictionary
            for (int i = 0;i<cols; i++)
            {
                for (int j = 0;j<rows; j++)
                {
                    GridPosition position = new(i, j);
                    databaseManager.AddTile(position);
                    checkTileDictionary();
                }
            }
            // Debug.Log("Position:" + position.rows + " Einträge.");


            //register adjecent tiles

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
}
