using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
//This class holds complex datatypes that are used across other scripts such as within the databasemanager

/*public class GridPosition
{
    public int column { get; set; }
    public int row { get; set; }
    public GridPosition(int column, int row)
    {
        this.column = column;
        this.row = row;

    }
}*/

[Serializable]
public class ObjectIdentifier
{
    // Static dictionaries can remain as-is; they're not serialized for the instance.
    private static readonly Dictionary<ObjectType, char> TypeToPrefix = new Dictionary<ObjectType, char>
    {
        { ObjectType.Tile,       '$' },
        { ObjectType.CityCenter, '#' },
        { ObjectType.Building,   '+' },
        { ObjectType.Animal,     '~' }
    };

    private static readonly Dictionary<char, ObjectType> PrefixToType =
        TypeToPrefix.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

    // These fields get serialized, which makes them visible in the Inspector
    [SerializeField] private ObjectType type;
    [SerializeField] private string uniqueId;

    // Public read-only properties that refer to the serialized fields
    public ObjectType Type => type;
    public string UniqueId => uniqueId;

    // This constructor is used in code
    public ObjectIdentifier(ObjectType type, string uniqueId)
    {
        this.type = type;
        this.uniqueId = uniqueId;
    }

    // Parameterless constructor so Unity can create it if needed
    // (Depending on usage, might not be strictly necessary, but often helpful)
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
    public Vector2Int Position { get; set; } // GridPosition für die Position des Tiles
    public GameObject TileObject { get; set; } // Referenz auf das GameObject des Tiles
    public TileType Type { get; set; } // Typ des Tiles
    public ResourceType Resource { get; set; } // Ressourcentyp des Tiles
    public List<Vector2Int> AdjacentTilesPosition { get; set; } // Liste von angrenzenden Tile-Positionen
    public List<ObjectIdentifier> ConstructionClaims { get; set; } // Liste von Bauansprüchen
    public DBTileValue(Vector2Int position, GameObject tileObject, TileType type, ResourceType resource)
    {
        Position = position;
        TileObject = tileObject;
        Type = type;
        Resource = resource;
        AdjacentTilesPosition = new List<Vector2Int>();
        ConstructionClaims = new List<ObjectIdentifier>();
    }

}

public class DBCityCenterValue
{
    public ObjectIdentifier _parentTile;
    public GameObject _object;
    public Dictionary<ProductType, int> _inventory;
    public int _housingLimit;
    public string _cityName;
}

public class DBBuildingValue
{
    public ObjectIdentifier _parentTile;
    public ObjectIdentifier _parentCityCenter;
    public BuildingType _type;
    public GameObject _object;
}

public class DBAnimalValue
{
    public ObjectIdentifier _parentCityCenter;
    public ObjectIdentifier _parentBuilding;
    public AnimalType _type;
    public string _animalName;
    public int priority;
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
    public int columns { get; set; } // Number of tile columns
    public int rows { get; set; } // Number of tile rows
    public float cellSize {  get; set; }

}

// Define a class to hold resource data, representing the resources that can spawn on a tile
[System.Serializable]
public class ResourceProbability
{
    [Tooltip("The type of resource that can spawn on this tile, such as 'Herbs' or 'Raw Rice'.")]
    public ResourceType resourceName;      // Name of the resource, e.g., "Herbs", "Raw Rice". See Enums class

    [Tooltip("The prefab used to visually represent this resource on the tile.")]
    public GameObject resourcePrefab;  // Prefab representing the resource (e.g., a plant or mineral)

    [Tooltip("Probability (0 to 1) of this resource spawning on the tile.")]
    [Range(0, 1)]
    public float spawnProbability;     // Probability of this resource spawning on the tile (0 to 1 range)
}

[System.Serializable]
public class ProductRequirement
{
    [Tooltip("The type of product required for production.")]
    public ProductType product;

    [Tooltip("The quantity of the product required for production.")]
    public int quantity;
}


