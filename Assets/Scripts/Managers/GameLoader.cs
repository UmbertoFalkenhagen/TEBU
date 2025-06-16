using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoader : MonoBehaviour
{

    private DatabaseManager databaseManager;
    public GameObject uiPrefab;
    public List<ScriptableBuilding> buildingBlueprints;
    public List<ScriptableResource> resourceBlueprints;

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

        foreach (ScriptableResource blueprint in resourceBlueprints)
        {
            if (blueprint == null)
            {
                Debug.LogWarning("Encountered a null ScriptableResource in resourceBlueprints. Skipping.");
                continue;
            }

            DatabaseManager.Instance.AddResourceBlueprint(blueprint);
        }

        Debug.Log("Finished populating buildingBlueprintDictionary from buildingBlueprints.");

        databaseManager.initMapData.columns = 5;
        databaseManager.initMapData.rows = 5;
        databaseManager.initMapData.cellSize = 1.24f;

        loadUI();
        //Run Generate Map
        //TODO: Check if any is there

        HexMapManager.Instance.GenerateMap();            


        //Load UI


        // ....

    }

    public void loadUI()
    {
        if (GameObject.Find("UICanvas") == null)
        {
            Instantiate(uiPrefab);
        }
    }
}
