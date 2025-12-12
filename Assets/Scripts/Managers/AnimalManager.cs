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

    public void AssignAnimalToBuilding(ObjectIdentifier animalId, ObjectIdentifier buildingId)
    {
        if (!DatabaseManager.Instance.AnimalDictionary.TryGetValue(animalId, out var animalValue))
        {
            Debug.LogError($"AnimalManager: Animal {animalId} not found");
            return;
        }

        if (!DatabaseManager.Instance.BuildingDictionary.ContainsKey(buildingId))
        {
            Debug.LogError($"AnimalManager: Building {buildingId} not found");
            return;
        }

        animalValue._assignedBuilding = buildingId;

        if (animalValue._object != null)
        {
            Animal animalComponent = animalValue._object.GetComponent<Animal>();
            if (animalComponent != null)
            {
                animalComponent.assignedBuilding = buildingId;
            }
        }

        Debug.Log($"AnimalManager: Assigned animal {animalId} to building {buildingId}");
    }
}
