using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingFactory : MonoBehaviour, IFactory<ScriptableBuilding>
{
    public static BuildingFactory Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameObject CreateObject(ScriptableBuilding data, GameObject positionObject, Quaternion rotation, GameObject parentObject)
    {
        if (data == null)
        {
            Debug.LogError("Building data is null. Cannot create building.");
            return null;
        }

        // Instantiate the basic building prefab
        GameObject buildingObject = Instantiate(data.basicPrefab, positionObject.transform.position, Quaternion.Euler(0, Random.Range(0f, 360f), 0), parentObject.transform); // Random rotation around y-axis

        if (buildingObject == null)
        {
            Debug.LogError("Failed to instantiate building prefab.");
            return null;
        }

        // Add and initialize building component
        Building buildingComponent = buildingObject.AddComponent<Building>();
        buildingComponent.Initialize(data);

        return buildingObject;
    }
}
