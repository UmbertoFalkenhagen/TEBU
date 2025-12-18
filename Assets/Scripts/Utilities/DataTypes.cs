using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

[Serializable]
public class ObjectIdentifier
{
    private static readonly Dictionary<ObjectType, char> TypeToPrefix = new Dictionary<ObjectType, char>
    {
        { ObjectType.Tile,       '$' },
        { ObjectType.CityCenter, '#' },
        { ObjectType.Building,   '+' },
        { ObjectType.Animal,     '~' }
    };

    private static readonly Dictionary<char, ObjectType> PrefixToType =
        TypeToPrefix.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

    [SerializeField] private ObjectType type;
    [SerializeField] private string uniqueId;

    public ObjectType Type => type;
    public string UniqueId => uniqueId;

    public ObjectIdentifier(ObjectType type, string uniqueId)
    {
        this.type = type;
        this.uniqueId = uniqueId;
    }

    private ObjectIdentifier() { }

    public override string ToString()
    {
        return $"{TypeToPrefix[type]}{uniqueId}";
    }

    public static ObjectIdentifier FromString(string objectIdString)
    {
        if (string.IsNullOrWhiteSpace(objectIdString))
            throw new ArgumentException("Object ID string cannot be null or empty.");

        char prefix = objectIdString[0];
        if (!PrefixToType.TryGetValue(prefix, out var parsedType))
        {
            throw new ArgumentException($"Unrecognized prefix '{prefix}' in object ID.");
        }

        string parsedId = objectIdString.Substring(1);
        return new ObjectIdentifier(parsedType, parsedId);
    }
}

public class DBTileValue
{
    public Vector2Int Position { get; set; }
    public GameObject TileObject { get; set; }
    public TileType Type { get; set; }
    public ResourceType Resource { get; set; }
    public List<Vector2Int> AdjacentTilesPosition { get; set; }
    public List<ObjectIdentifier> ConstructionClaims { get; set; }
    public List<ObjectIdentifier> ActiveClaims { get; set; }

    public DBTileValue(Vector2Int position, GameObject tileObject, TileType type, ResourceType resource)
    {
        Position = position;
        TileObject = tileObject;
        Type = type;
        Resource = resource;
        AdjacentTilesPosition = new List<Vector2Int>();
        ConstructionClaims = new List<ObjectIdentifier>();
        ActiveClaims = new List<ObjectIdentifier>();
    }
}

public class DBCityCenterValue
{
    public ObjectIdentifier _parentTile;
    public GameObject _object;
    public Dictionary<ProductType, int> _inventory;
    public int _housingLimit;
    public string _cityName;

    public List<ObjectIdentifier> _buildings;
    public List<ObjectIdentifier> _animals;

    public DBCityCenterValue(ObjectIdentifier parentTile, GameObject obj, Dictionary<ProductType, int> inventory, int housingLimit, string cityName)
    {
        _parentTile = parentTile;
        _object = obj;
        _inventory = inventory;
        _housingLimit = housingLimit;
        _cityName = cityName;
        _buildings = new List<ObjectIdentifier>();
        _animals = new List<ObjectIdentifier>();
    }
}

public class DBBuildingValue
{
    public ObjectIdentifier _parentTile;
    public BuildingType _type;
    public GameObject _object;
    public List<ObjectIdentifier> _workers;
    public List<ObjectIdentifier> _claimedTiles;
    public int _currentAnimalWorkerLimit;

    public DBBuildingValue(ObjectIdentifier parentTile, BuildingType buildingType, GameObject obj)
    {
        _parentTile = parentTile;
        _type = buildingType;
        _object = obj;
        _workers = new List<ObjectIdentifier>();
        _claimedTiles = new List<ObjectIdentifier>();
        _currentAnimalWorkerLimit = 0;
    }

    public int GetCurrentWorkerCount()
    {
        return _workers != null ? _workers.Count : 0;
    }
}

public class DBAnimalValue
{
    public ObjectIdentifier _parentCityCenter;
    public ObjectIdentifier _parentBuilding;
    public GameObject _object;
    public AnimalType _type;
    public string _animalName;
    public int priority;

    public DBAnimalValue(ObjectIdentifier parentCityCenter, AnimalType type, string animalName, int priority, GameObject obj)
    {
        _parentCityCenter = parentCityCenter;
        _parentBuilding = null;
        _object = obj;
        _type = type;
        this._animalName = animalName;
        this.priority = priority;
    }
}

public class SDBAnimalBlueprintValue
{
    public GameObject prefab;
    public List<TileType> requiredTileTypes;
    public ProductType basicFood;
    public ProductType ability1UnlockProduct;
    public ProductType ability2UnlockProduct;
    public ProductType ability1ImprovProduct;
    public ProductType ability2ImprovProduct;
}

public class SDBBuildingBlueprintValue
{
    public GameObject prefab;
    public GameObject unworkedModulePrefab;
    public GameObject workedModulePrefab;
    public List<TileType> requiredTileTypes;
    public List<ResourceType> requiredResources;
    public ProductType outputProduct;
    public List<ProductType> inputProducts;
    public int productionPerWorker;
    public bool isMaxWorkersFixed;
    public int maxWorkers;
}

public class SDBInitMapData
{
    public int columns { get; set; }
    public int rows { get; set; }
    public float cellSize { get; set; }
}

[System.Serializable]
public class ResourceProbability
{
    [Tooltip("The type of resource that can spawn on this tile, such as 'Herbs' or 'Raw Rice'.")]
    public ResourceType resourceName;

    [Tooltip("Probability (0 to 1) of this resource spawning on the tile.")]
    [Range(0, 1)]
    public float spawnProbability;
}

[System.Serializable]
public class ProductRequirement
{
    [Tooltip("The type of product required for production.")]
    public ProductType product;

    [Tooltip("The quantity of the product required for production.")]
    public int quantity;
}

[System.Serializable]
public class TileSpawnConfig
{
    [Tooltip("The ScriptableTile blueprint to spawn")]
    public ScriptableTile tileBlueprint;

    [Tooltip("Spawn probability (0-100). All probabilities in HexMapManager must sum to 100")]
    [Range(0f, 100f)]
    public float spawnProbability = 0f;
}
