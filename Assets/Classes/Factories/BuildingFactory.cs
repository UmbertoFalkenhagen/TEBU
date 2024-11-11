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
            Debug.LogError("CityCenter data is null. Cannot create city center.");
            return null;
        }

        // Instantiate the basic city center prefab
        GameObject buildingObject = Instantiate(data.basicPrefab, positionObject.transform.position, rotation, parentObject.transform);
        if (buildingObject == null)
        {
            Debug.LogError("Failed to instantiate CityCenter prefab.");
            return null;
        }

        // Add and initialize Building component

        return buildingObject;
    }
}
