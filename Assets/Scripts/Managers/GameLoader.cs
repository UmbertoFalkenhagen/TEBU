using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoader : MonoBehaviour
{

    private DatabaseManager databaseManager;

    public List<ScriptableBuilding> buildingBlueprints;

    // Start is called before the first frame update
    void Start()
    {
        if (databaseManager == null)
        {
            databaseManager = DatabaseManager.Instance;
            if (databaseManager == null)
            {
                Debug.LogError("DatabaseManager could not be found!");
            }
        }

        // Loop through each ScriptableBuilding in the list
        foreach (ScriptableBuilding blueprint in buildingBlueprints)
        {
            if (blueprint == null)
            {
                Debug.LogWarning("Encountered a null ScriptableBuilding in buildingBlueprints. Skipping.");
                continue;
            }

            // For each valid ScriptableBuilding, call the DatabaseManager's function
            DatabaseManager.Instance.AddBuildingBlueprint(blueprint);
        }

        Debug.Log("Finished populating buildingBlueprintDictionary from buildingBlueprints.");

        databaseManager.initMapData.columns = 5;
        databaseManager.initMapData.rows = 5;
        databaseManager.initMapData.cellSize = 1.24f;


        //Run Generate Map
        //TODO: Check if any is there

        HexMapManager.Instance.GenerateMap();            


        //Load UI


        // ....

    }
}
