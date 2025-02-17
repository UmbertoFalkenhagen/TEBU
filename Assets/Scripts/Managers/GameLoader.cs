using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoader : MonoBehaviour
{

    private HexMapGenerator hexMapGenerator;
    private DatabaseManager databaseManager;

    // Start is called before the first frame update
    void Start()
    {
        if (databaseManager == null)
        {
            databaseManager = FindObjectOfType<DatabaseManager>();
            if (databaseManager == null)
            {
                Debug.LogError("DatabaseManager could not be found!");
            }
        }
    
        //fillBlueprintDB
        databaseManager.initMapData.columns = 5;
        databaseManager.initMapData.rows = 5;


        //Run Generate Map
        //TODO: Check if any is there
        if (hexMapGenerator == null) { 
            hexMapGenerator = FindObjectOfType<HexMapGenerator>();
            
        }
        hexMapGenerator.GenerateMap();

        //Load UI


        // ....

    }
}
