using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityCenterFactory : MonoBehaviour, IFactory<ScriptableCityCenter>
{
    public static CityCenterFactory Instance;

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

    public GameObject CreateObject(ScriptableCityCenter data, GameObject positionObject, Quaternion rotation, GameObject parentObject)
    {
        if (data == null)
        {
            Debug.LogError("CityCenter data is null. Cannot create city center.");
            return null;
        }

        // Instantiate the basic city center prefab
        GameObject cityCenterObject = Instantiate(data.basicPrefab, positionObject.transform.position, rotation, parentObject.transform);
        if (cityCenterObject == null)
        {
            Debug.LogError("Failed to instantiate CityCenter prefab.");
            return null;
        }

        // Add and initialize CityCenter component
        CityCenter cityCenterComponent = cityCenterObject.AddComponent<CityCenter>();
        cityCenterComponent.Initialize(data);

        return cityCenterObject;
    }
}
