using System.Collections.Generic;
using UnityEngine;

public class AnimalFactory : MonoBehaviour
{
    public static AnimalFactory Instance;

    private const string SPAWN_POINT_NAME = "AnimalSpawnPoint";

    private List<string> animalNames = new List<string>
    {
        "Luna", "Max", "Bella", "Charlie", "Lucy", "Cooper", "Daisy", "Milo",
        "Sadie", "Rocky", "Molly", "Buddy", "Maggie", "Jack", "Sophie", "Duke",
        "Chloe", "Bear", "Lola", "Zeus", "Zoe", "Oliver", "Lily", "Tucker",
        "Stella", "Bentley", "Nala", "Leo", "Penny", "Finn", "Rosie", "Winston",
        "Ruby", "Murphy", "Bailey", "Jasper", "Ellie", "Teddy", "Gracie", "Gus",
        "Willow", "Louie", "Zoey", "Moose", "Coco", "Oscar", "Harley", "Remi"
    };

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

    public KeyValuePair<ObjectIdentifier, DBAnimalValue> CreateAnimal(
        AnimalType animalType,
        ObjectIdentifier parentCityCenterId,
        int priority = 0)
    {
        SDBAnimalBlueprintValue animalBlueprint = DatabaseManager.Instance.animalBlueprintDictionary.TryGetValue(animalType, out var blueprint)
            ? blueprint
            : null;

        if (animalBlueprint == null)
        {
            Debug.LogError($"AnimalFactory: No blueprint found for AnimalType '{animalType}'");
            return default;
        }

        if (!DatabaseManager.Instance.CityCenterDictionary.TryGetValue(parentCityCenterId, out var cityCenter))
        {
            Debug.LogError($"AnimalFactory: Parent city center '{parentCityCenterId}' not found in database");
            return default;
        }

        DBTileValue parentTile = DatabaseManager.Instance.GetTileValue(cityCenter._parentTile);
        if (parentTile == null || parentTile.TileObject == null)
        {
            Debug.LogError($"AnimalFactory: Parent tile not found for city center '{parentCityCenterId}'");
            return default;
        }

        GameObject cityCenterInstance = parentTile.TileObject.GetComponent<HexTile>().heldBuilding;
        if (cityCenterInstance == null)
        {
            Debug.LogError($"AnimalFactory: City center instance not found on tile");
            return default;
        }

        Transform spawnPoint = cityCenterInstance.transform.Find(SPAWN_POINT_NAME);
        Vector3 spawnPosition;
        Transform parentTransform;

        if (spawnPoint != null)
        {
            spawnPosition = spawnPoint.position;
            parentTransform = spawnPoint;
        }
        else
        {
            Debug.LogWarning($"AnimalFactory: '{SPAWN_POINT_NAME}' not found in city center, using city center position");
            spawnPosition = cityCenterInstance.transform.position;
            parentTransform = cityCenterInstance.transform;
        }

        ObjectIdentifier animalID = DatabaseManager.Instance.GenerateUniqueId(ObjectType.Animal);

        GameObject animalObject = null;
        if (animalBlueprint.prefab != null)
        {
            animalObject = Instantiate(animalBlueprint.prefab, spawnPosition, animalBlueprint.prefab.transform.rotation, parentTransform);

            Animal animalComponent = animalObject.GetComponent<Animal>();
            if (animalComponent == null)
            {
                animalComponent = animalObject.AddComponent<Animal>();
            }
            animalComponent.animalID = animalID;
            animalComponent.animalType = animalType;
        }

        string animalName = GetAndRemoveRandomAnimalName();

        DBAnimalValue animalValue = new DBAnimalValue(parentCityCenterId, animalType, animalName, priority, animalObject);

        return new KeyValuePair<ObjectIdentifier, DBAnimalValue>(animalID, animalValue);
    }

    private string GetAndRemoveRandomAnimalName()
    {
        if (animalNames.Count == 0)
        {
            Debug.LogWarning("Animal name list is empty! Generating random name.");
            return $"Animal_{Random.Range(1000, 9999)}";
        }

        int randomIndex = Random.Range(0, animalNames.Count);
        string selectedName = animalNames[randomIndex];
        animalNames.RemoveAt(randomIndex);

        return selectedName;
    }
}
