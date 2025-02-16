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


        if (databaseManager == null)
        {
            databaseManager = FindObjectOfType<DatabaseManager>();
        }

        //Fill with data from SDBInitMapData
        // seed = "abc";
        rows = 0;
        cols = 0;
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


            //TODO: Add Tile to Dictionary
            GridPosition position = new(cols, rows);

            //
            databaseManager.AddTile(position);
            checkTileDictionary();

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
