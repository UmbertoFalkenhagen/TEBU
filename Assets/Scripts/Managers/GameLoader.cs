using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoader : MonoBehaviour
{

    private DatabaseManager databaseManager;
    public GameObject uiPrefab;
    public List<ScriptableBuilding> buildingBlueprints;
    public List<ScriptableResource> resourceBlueprints;
    public List<ScriptableAnimal> animalBlueprints;

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

        foreach (ScriptableBuilding blueprint in buildingBlueprints)
        {
            if (blueprint == null)
            {
                Debug.LogWarning("Encountered a null ScriptableBuilding in buildingBlueprints. Skipping.");
                continue;
            }

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

        foreach (ScriptableAnimal blueprint in animalBlueprints)
        {
            if (blueprint == null)
            {
                Debug.LogWarning("Encountered a null ScriptableAnimal in animalBlueprints. Skipping.");
                continue;
            }

            DatabaseManager.Instance.AddAnimalBlueprint(blueprint);
        }

        Debug.Log("Finished populating buildingBlueprintDictionary from buildingBlueprints.");
        Debug.Log($"Loaded {buildingBlueprints.Count} building blueprints, {resourceBlueprints.Count} resource blueprints, and {animalBlueprints.Count} animal blueprints.");

        databaseManager.initMapData.columns = 5;
        databaseManager.initMapData.rows = 5;
        databaseManager.initMapData.cellSize = 1.24f;

        loadUI();

        HexMapManager.Instance.GenerateMap();
    }

    public void loadUI()
    {
        if (GameObject.Find("UICanvas") == null)
        {
            Instantiate(uiPrefab);
        }
    }
}
