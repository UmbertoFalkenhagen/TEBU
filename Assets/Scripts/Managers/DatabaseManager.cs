// Full updated DatabaseManager.cs
// Reflects the change where buildings and animals no longer store their city center
// Instead, the DBCityCenterValue keeps track of its buildings and animals

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance;

    public List<ScriptableTile> tileBlueprints = new List<ScriptableTile>();

    private Dictionary<ObjectIdentifier, DBTileValue> tileDictionary = new();
    public IReadOnlyDictionary<ObjectIdentifier, DBTileValue> TileDictionary => tileDictionary;

    private Dictionary<ObjectIdentifier, DBCityCenterValue> cityCenterDictionary = new();
    public IReadOnlyDictionary<ObjectIdentifier, DBCityCenterValue> CityCenterDictionary => cityCenterDictionary;

    private Dictionary<ObjectIdentifier, DBBuildingValue> buildingDictionary = new();
    public IReadOnlyDictionary<ObjectIdentifier, DBBuildingValue> BuildingDictionary => buildingDictionary;

    private Dictionary<ObjectIdentifier, DBAnimalValue> animalDictionary = new();
    public IReadOnlyDictionary<ObjectIdentifier, DBAnimalValue> AnimalDictionary => animalDictionary;

    public Dictionary<BuildingType, SDBBuildingBlueprintValue> buildingBlueprintDictionary = new();
    public Dictionary<AnimalType, SDBAnimalBlueprintValue> animalBlueprintDictionary = new();
    public Dictionary<ResourceType, GameObject> resourceBlueprintDictionary = new();

    public SDBInitMapData initMapData = new();

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #region General Functions
    public ObjectIdentifier GenerateUniqueId(ObjectType type)
    {
        const int ID_LENGTH = 6;
        while (true)
        {
            int max = (int)Mathf.Pow(10, ID_LENGTH);
            int randomNumber = UnityEngine.Random.Range(0, max);
            string uniqueIdPart = randomNumber.ToString($"D{ID_LENGTH}");
            ObjectIdentifier candidate = new(type, uniqueIdPart);

            bool exists = type switch
            {
                ObjectType.Tile => tileDictionary.ContainsKey(candidate),
                ObjectType.CityCenter => cityCenterDictionary.ContainsKey(candidate),
                ObjectType.Building => buildingDictionary.ContainsKey(candidate),
                ObjectType.Animal => animalDictionary.ContainsKey(candidate),
                _ => true
            };

            if (!exists) return candidate;
        }
    }

    public object FindObject(ObjectIdentifier identifier)
    {
        return identifier.Type switch
        {
            ObjectType.Tile => tileDictionary.TryGetValue(identifier, out var tVal) ? tVal : null,
            ObjectType.CityCenter => cityCenterDictionary.TryGetValue(identifier, out var cVal) ? cVal : null,
            ObjectType.Building => buildingDictionary.TryGetValue(identifier, out var bVal) ? bVal : null,
            ObjectType.Animal => animalDictionary.TryGetValue(identifier, out var aVal) ? aVal : null,
            _ => null
        };
    }

    public ObjectIdentifier GetStructureId(ObjectIdentifier tileId)
    {
        return GetCityCenterIdByTileId(tileId) ?? GetBuildingIdByTileId(tileId);
    }
    #endregion

    #region Tile Functions
    public void AddTile(ObjectIdentifier tileID, DBTileValue tileValue)
    {
        if (!tileDictionary.ContainsKey(tileID))
            tileDictionary[tileID] = tileValue;
    }

    public DBTileValue GetTileValue(ObjectIdentifier tileID)
    {
        return tileID.Type == ObjectType.Tile && tileDictionary.TryGetValue(tileID, out var val) ? val : null;
    }

    public ResourceType GetTileResourceType(ObjectIdentifier tileID) => GetTileValue(tileID)?.Resource ?? ResourceType.None;

    public GameObject GetTileGameObject(ObjectIdentifier tileID) => GetTileValue(tileID)?.TileObject;

    public ObjectIdentifier GetTileIdByObject(GameObject tileObj)
    {
        return tileDictionary.FirstOrDefault(kvp => kvp.Value.TileObject == tileObj).Key;
    }

    public List<DBTileValue> GetTileNeighbors(ObjectIdentifier tileID)
    {
        var val = GetTileValue(tileID);
        return val?.AdjacentTilesPosition.Select(pos => tileDictionary.Values.FirstOrDefault(t => t.Position == pos)).Where(n => n != null).ToList();
    }
    #endregion

    #region City Center
    public void AddCityCenter(ObjectIdentifier cityCenterID, DBCityCenterValue value)
    {
        if (!cityCenterDictionary.ContainsKey(cityCenterID))
            cityCenterDictionary[cityCenterID] = value;
    }

    public void RemoveCityCenter(ObjectIdentifier cityCenterID)
    {
        cityCenterDictionary.Remove(cityCenterID);
    }

    public DBCityCenterValue GetCityCenterValue(ObjectIdentifier cityCenterID)
    {
        return cityCenterID.Type == ObjectType.CityCenter && cityCenterDictionary.TryGetValue(cityCenterID, out var val) ? val : null;
    }

    public ObjectIdentifier GetCityCenterIdByTileId(ObjectIdentifier tileId)
    {
        return cityCenterDictionary.FirstOrDefault(kvp => kvp.Value._parentTile == tileId).Key;
    }

    public ObjectIdentifier GetParentTileIdByCityCenterId(ObjectIdentifier cityCenterId)
    {
        return cityCenterDictionary.TryGetValue(cityCenterId, out var val) ? val._parentTile : null;
    }

    public List<ObjectIdentifier> GetBuildingIdsByCityCenterId(ObjectIdentifier cityCenterId)
    {
        return cityCenterDictionary.TryGetValue(cityCenterId, out var cityCenter) ? new List<ObjectIdentifier>(cityCenter._buildings) : new();
    }

    public List<ObjectIdentifier> GetAnimalIdsByCityCenterId(ObjectIdentifier cityCenterId)
    {
        return cityCenterDictionary.TryGetValue(cityCenterId, out var cityCenter) ? new List<ObjectIdentifier>(cityCenter._animals) : new();
    }
    #endregion

    #region Building
    public void AddBuilding(ObjectIdentifier buildingID, DBBuildingValue value, ObjectIdentifier parentCityCenterId)
    {
        if (!buildingDictionary.ContainsKey(buildingID))
        {
            buildingDictionary[buildingID] = value;
            if (cityCenterDictionary.TryGetValue(parentCityCenterId, out var cityCenter))
            {
                cityCenter._buildings.Add(buildingID);
            }
        }
    }

    public DBBuildingValue GetBuildingValue(ObjectIdentifier buildingID)
    {
        return buildingID.Type == ObjectType.Building && buildingDictionary.TryGetValue(buildingID, out var val) ? val : null;
    }

    public ObjectIdentifier GetBuildingIdByTileId(ObjectIdentifier tileId)
    {
        return buildingDictionary.FirstOrDefault(kvp => kvp.Value._parentTile == tileId).Key;
    }

    public ObjectIdentifier GetParentTileIdByBuildingId(ObjectIdentifier buildingId)
    {
        return buildingDictionary.TryGetValue(buildingId, out var val) ? val._parentTile : null;
    }

    public ObjectIdentifier GetCityCenterIdByBuildingId(ObjectIdentifier buildingId)
    {
        return cityCenterDictionary.FirstOrDefault(kvp => kvp.Value._buildings.Contains(buildingId)).Key;
    }
    #endregion

    #region Animal
    public void AddAnimal(ObjectIdentifier animalID, DBAnimalValue value, ObjectIdentifier parentCityCenterId)
    {
        if (!animalDictionary.ContainsKey(animalID))
        {
            animalDictionary[animalID] = value;
            if (cityCenterDictionary.TryGetValue(parentCityCenterId, out var cityCenter))
            {
                cityCenter._animals.Add(animalID);
            }
        }
    }

    #endregion

    #region Static Blueprint DB
    public void AddBuildingBlueprint(ScriptableBuilding scriptableBuilding)
    {
        if (scriptableBuilding == null) return;

        SDBBuildingBlueprintValue blueprint = new()
        {
            prefab = scriptableBuilding.basicPrefab,
            unworkedModulePrefab = scriptableBuilding.emptyModulePrefab,
            workedModulePrefab = scriptableBuilding.workedModulePrefab,
            requiredTileTypes = new(scriptableBuilding.suitableTileTypeLocations),
            requiredResources = new(scriptableBuilding.requiredResources),
            outputProduct = scriptableBuilding.product,
            inputProducts = scriptableBuilding.inputProducts.Select(req => req.product).ToList(),
            productionPerWorker = scriptableBuilding.productionPerWorker,
            isMaxWorkersFixed = scriptableBuilding.isMaxWorkersFixed,
            maxWorkers = scriptableBuilding.maxWorkers
        };

        buildingBlueprintDictionary[scriptableBuilding.buildingName] = blueprint;
    }

    public void AddAnimalBlueprint(ScriptableAnimal scriptableAnimal)
    {
        if (scriptableAnimal == null) return;

        SDBAnimalBlueprintValue blueprint = new()
        {
            prefab = scriptableAnimal.prefab,
            requiredTileTypes = new List<TileType> { scriptableAnimal.spawnLocation },
            basicFood = scriptableAnimal.basicFood,
            ability1UnlockProduct = scriptableAnimal.basicAbilityUnlockProduct1,
            ability2UnlockProduct = scriptableAnimal.basicAbilityUnlockProduct2,
            ability1ImprovProduct = scriptableAnimal.abilityImprovementProduct1,
            ability2ImprovProduct = scriptableAnimal.abilityImprovementProduct2
        };

        animalBlueprintDictionary[scriptableAnimal.animalName] = blueprint;
    }

    public void AddResourceBlueprint(ScriptableResource scriptableResource)
    {
        if (scriptableResource != null)
            resourceBlueprintDictionary[scriptableResource.resourcename] = scriptableResource.resourcePrefab;
    }

    public GameObject GetResourcePrefabForType(ResourceType type)
    {
        return resourceBlueprintDictionary.TryGetValue(type, out var prefab) ? prefab : null;
    }

    public SDBBuildingBlueprintValue GetBuildingBlueprint(BuildingType type)
    {
        return buildingBlueprintDictionary.TryGetValue(type, out var blueprint) ? blueprint : null;
    }

    public SDBBuildingBlueprintValue GetBuildingBlueprint(string typeName)
    {
        return Enum.TryParse<BuildingType>(typeName, true, out var parsedType) ? GetBuildingBlueprint(parsedType) : null;
    }
    #endregion
}
