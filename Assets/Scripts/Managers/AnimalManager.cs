using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AnimalManager : MonoBehaviour
{
    public static AnimalManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #region Complex Animal Operations

    public Animal SpawnAnimal(AnimalType animalType, ObjectIdentifier cityCenterId, int priority = 0)
    {
        if (!DatabaseManager.Instance.CityCenterDictionary.ContainsKey(cityCenterId))
        {
            Debug.LogError($"AnimalManager: City center {cityCenterId} not found in database");
            return null;
        }

        KeyValuePair<ObjectIdentifier, DBAnimalValue> animalPair =
            AnimalFactory.Instance.CreateAnimal(animalType, cityCenterId, priority);

        if (animalPair.Key == null || animalPair.Value == null)
        {
            Debug.LogError($"AnimalManager: Failed to create animal of type {animalType}");
            return null;
        }

        DatabaseManager.Instance.AddAnimal(animalPair.Key, animalPair.Value, cityCenterId);

        Debug.Log($"AnimalManager: Successfully spawned {animalType} ({animalPair.Value._animalName}) in city {cityCenterId}");

        if (animalPair.Value._object != null)
        {
            return animalPair.Value._object.GetComponent<Animal>();
        }

        return null;
    }

    public Animal SpawnRandomAnimalForCity(ObjectIdentifier cityCenterId)
    {
        if (!DatabaseManager.Instance.CityCenterDictionary.TryGetValue(cityCenterId, out var cityCenter))
        {
            Debug.LogError($"AnimalManager: City center {cityCenterId} not found");
            return null;
        }

        ObjectIdentifier parentTileId = cityCenter._parentTile;
        DBTileValue tileValue = DatabaseManager.Instance.GetTileValue(parentTileId);

        if (tileValue == null)
        {
            Debug.LogError($"AnimalManager: Parent tile not found for city {cityCenterId}");
            return null;
        }

        List<AnimalType> availableAnimals = GetAvailableAnimalsForTileType(tileValue.Type);

        if (availableAnimals.Count == 0)
        {
            Debug.LogWarning($"AnimalManager: No animals available for tile type {tileValue.Type}");
            return null;
        }

        AnimalType randomAnimal = availableAnimals[Random.Range(0, availableAnimals.Count)];
        return SpawnAnimal(randomAnimal, cityCenterId);
    }

    public void AssignAnimalToBuilding(ObjectIdentifier animalId, ObjectIdentifier buildingId)
    {
        if (!DatabaseManager.Instance.AnimalDictionary.TryGetValue(animalId, out var animalValue))
        {
            Debug.LogError($"AnimalManager: Animal {animalId} not found");
            return;
        }

        if (!DatabaseManager.Instance.BuildingDictionary.TryGetValue(buildingId, out var buildingValue))
        {
            Debug.LogError($"AnimalManager: Building {buildingId} not found");
            return;
        }

        ObjectIdentifier previousBuilding = animalValue._parentBuilding;

        if (previousBuilding != null && DatabaseManager.Instance.BuildingDictionary.TryGetValue(previousBuilding, out var prevBuildingValue))
        {
            prevBuildingValue._workers.Remove(animalId);
        }

        animalValue._parentBuilding = buildingId;
        buildingValue._workers.Add(animalId);

        if (animalValue._object != null)
        {
            if (buildingValue._object != null)
            {
                Transform spawnPoint = buildingValue._object.transform.Find("AnimalSpawnPoint");
                if (spawnPoint == null)
                {
                    spawnPoint = buildingValue._object.transform;
                }

                animalValue._object.transform.SetParent(spawnPoint);
                animalValue._object.transform.position = spawnPoint.position;
            }
        }

        Debug.Log($"AnimalManager: Assigned animal {animalId} ({animalValue._animalName}) to building {buildingId}");
    }

    public void UnassignAnimalFromBuilding(ObjectIdentifier animalId)
    {
        if (!DatabaseManager.Instance.AnimalDictionary.TryGetValue(animalId, out var animalValue))
        {
            Debug.LogError($"AnimalManager: Animal {animalId} not found");
            return;
        }

        ObjectIdentifier previousBuilding = animalValue._parentBuilding;
        if (previousBuilding != null && DatabaseManager.Instance.BuildingDictionary.TryGetValue(previousBuilding, out var buildingValue))
        {
            buildingValue._workers.Remove(animalId);
        }

        animalValue._parentBuilding = null;

        if (animalValue._object != null)
        {
            MoveAnimalToCitySpawnPoint(animalId, animalValue._parentCityCenter);
        }

        Debug.Log($"AnimalManager: Unassigned animal {animalId} from building");
    }

    public void MoveAnimalToCity(ObjectIdentifier animalId, ObjectIdentifier targetCityCenterId)
    {
        if (!DatabaseManager.Instance.AnimalDictionary.TryGetValue(animalId, out var animalValue))
        {
            Debug.LogError($"AnimalManager: Animal {animalId} not found");
            return;
        }

        if (!DatabaseManager.Instance.CityCenterDictionary.TryGetValue(targetCityCenterId, out var targetCityValue))
        {
            Debug.LogError($"AnimalManager: Target city center {targetCityCenterId} not found");
            return;
        }

        ObjectIdentifier previousCityId = animalValue._parentCityCenter;

        if (previousCityId != targetCityCenterId)
        {
            if (DatabaseManager.Instance.CityCenterDictionary.TryGetValue(previousCityId, out var previousCityValue))
            {
                previousCityValue._animals.Remove(animalId);
            }

            targetCityValue._animals.Add(animalId);
            animalValue._parentCityCenter = targetCityCenterId;
        }

        if (animalValue._parentBuilding != null)
        {
            UnassignAnimalFromBuilding(animalId);
        }
        else
        {
            MoveAnimalToCitySpawnPoint(animalId, targetCityCenterId);
        }

        Debug.Log($"AnimalManager: Moved animal {animalId} ({animalValue._animalName}) to city {targetCityCenterId}");
    }

    #endregion

    #region Helper Methods

    private List<AnimalType> GetAvailableAnimalsForTileType(TileType tileType)
    {
        List<AnimalType> availableAnimals = new List<AnimalType>();

        foreach (var kvp in DatabaseManager.Instance.animalBlueprintDictionary)
        {
            if (kvp.Value.requiredTileTypes.Contains(tileType))
            {
                availableAnimals.Add(kvp.Key);
            }
        }

        return availableAnimals;
    }

    private void MoveAnimalToCitySpawnPoint(ObjectIdentifier animalId, ObjectIdentifier cityCenterId)
    {
        if (!DatabaseManager.Instance.AnimalDictionary.TryGetValue(animalId, out var animalValue))
        {
            return;
        }

        if (!DatabaseManager.Instance.CityCenterDictionary.TryGetValue(cityCenterId, out var cityValue))
        {
            return;
        }

        if (animalValue._object != null)
        {
            DBTileValue cityTile = DatabaseManager.Instance.GetTileValue(cityValue._parentTile);
            if (cityTile != null && cityTile.TileObject != null)
            {
                HexTile hexTile = cityTile.TileObject.GetComponent<HexTile>();
                if (hexTile != null && hexTile.heldBuilding != null)
                {
                    Transform spawnPoint = hexTile.heldBuilding.transform.Find("AnimalSpawnPoint");
                    if (spawnPoint == null)
                    {
                        spawnPoint = hexTile.heldBuilding.transform;
                    }

                    animalValue._object.transform.SetParent(spawnPoint);
                    animalValue._object.transform.position = spawnPoint.position;
                }
            }
        }
    }

    public List<ObjectIdentifier> GetUnemployedAnimalsInCity(ObjectIdentifier cityCenterId)
    {
        List<ObjectIdentifier> unemployed = new List<ObjectIdentifier>();

        if (!DatabaseManager.Instance.CityCenterDictionary.TryGetValue(cityCenterId, out var cityValue))
        {
            return unemployed;
        }

        foreach (ObjectIdentifier animalId in cityValue._animals)
        {
            if (DatabaseManager.Instance.AnimalDictionary.TryGetValue(animalId, out var animalValue))
            {
                if (animalValue._parentBuilding == null)
                {
                    unemployed.Add(animalId);
                }
            }
        }

        return unemployed;
    }

    public List<ObjectIdentifier> GetUnemployedAnimalsInOtherCities(ObjectIdentifier excludeCityCenterId)
    {
        List<ObjectIdentifier> unemployed = new List<ObjectIdentifier>();

        foreach (var cityEntry in DatabaseManager.Instance.CityCenterDictionary)
        {
            if (cityEntry.Key == excludeCityCenterId)
            {
                continue;
            }

            foreach (ObjectIdentifier animalId in cityEntry.Value._animals)
            {
                if (DatabaseManager.Instance.AnimalDictionary.TryGetValue(animalId, out var animalValue))
                {
                    if (animalValue._parentBuilding == null)
                    {
                        unemployed.Add(animalId);
                    }
                }
            }
        }

        return unemployed;
    }

    public bool CanAcceptAnimalInCity(ObjectIdentifier cityCenterId)
    {
        if (!DatabaseManager.Instance.CityCenterDictionary.TryGetValue(cityCenterId, out var cityValue))
        {
            return false;
        }

        int currentPopulation = cityValue._animals != null ? cityValue._animals.Count : 0;
        return currentPopulation < cityValue._housingLimit;
    }

    public bool CanAcceptWorkerInBuilding(ObjectIdentifier buildingId)
    {
        if (!DatabaseManager.Instance.BuildingDictionary.TryGetValue(buildingId, out var buildingValue))
        {
            return false;
        }

        SDBBuildingBlueprintValue blueprint = DatabaseManager.Instance.GetBuildingBlueprint(buildingValue._type);
        if (blueprint == null)
        {
            return false;
        }

        int currentWorkers = buildingValue.GetCurrentWorkerCount();
        return currentWorkers < blueprint.maxWorkers;
    }

    public void PrintAnimalInfo(ObjectIdentifier animalID)
    {
        DBAnimalValue animalData = DatabaseManager.Instance.AnimalDictionary.TryGetValue(animalID, out var data)
            ? data
            : null;

        if (animalData == null)
        {
            Debug.LogWarning($"AnimalManager: No database entry found for {animalID}");
            return;
        }

        string parentCityName = "Unknown City";
        if (DatabaseManager.Instance.CityCenterDictionary.TryGetValue(animalData._parentCityCenter, out var cityCenter))
        {
            parentCityName = cityCenter._cityName;
        }

        string jobStatus = "Unemployed";
        if (animalData._parentBuilding != null)
        {
            DBBuildingValue buildingData = DatabaseManager.Instance.GetBuildingValue(animalData._parentBuilding);
            if (buildingData != null)
            {
                jobStatus = $"Working at {buildingData._type}";
            }
        }

        Debug.Log($"?? Hello! I'm {animalData._animalName}, a {animalData._type} (ID: {animalID})\n" +
                  $"   ??? Living in: {parentCityName}\n" +
                  $"   ?? Job: {jobStatus}\n" +
                  $"   ? Priority: {animalData.priority}");
    }

    #endregion

    #region Database Wrapper Methods for Subordinate Components

    public DBAnimalValue GetAnimalValue(ObjectIdentifier animalId)
    {
        return DatabaseManager.Instance.GetAnimalValue(animalId);
    }

    public ObjectIdentifier GetCityCenterIdByAnimalId(ObjectIdentifier animalId)
    {
        return DatabaseManager.Instance.GetCityCenterIdByAnimalId(animalId);
    }

    public ObjectIdentifier GetBuildingIdByAnimalId(ObjectIdentifier animalId)
    {
        return DatabaseManager.Instance.GetBuildingIdByAnimalId(animalId);
    }

    public SDBAnimalBlueprintValue GetAnimalBlueprint(AnimalType animalType)
    {
        return DatabaseManager.Instance.GetAnimalBlueprint(animalType);
    }

    #endregion
}
