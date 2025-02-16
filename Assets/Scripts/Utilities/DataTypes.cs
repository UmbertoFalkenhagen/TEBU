using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
//This class holds complex datatypes that are used across other scripts such as within the databasemanager

public class GridPosition
{
    private int column;
    private int row;
    public GridPosition(int column, int row)
    {
        this.column = column;
        this.row = row;
    }
}

public class ObjectIdentifier
{
    // Lookup from enum -> prefix character
    private static readonly Dictionary<ObjectType, char> TypeToPrefix =
        new Dictionary<ObjectType, char>
        {
            { ObjectType.Tile,       '$' },
            { ObjectType.CityCenter, '#' },
            { ObjectType.Building,   '+' },
            { ObjectType.Animal,     '~' }
        };

    // Reverse lookup from prefix -> enum
    private static readonly Dictionary<char, ObjectType> PrefixToType =
        TypeToPrefix.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

    public ObjectType Type { get; }
    public string UniqueId { get; }

    public ObjectIdentifier(ObjectType type, string uniqueId)
    {
        Type = type;
        UniqueId = uniqueId;
    }

    public override string ToString()
    {
        return $"{TypeToPrefix[Type]}{UniqueId}";
    }

    public static ObjectIdentifier FromString(string objectIdString)
    {
        if (string.IsNullOrWhiteSpace(objectIdString))
            throw new ArgumentException("Object ID string cannot be null or empty.");

        char prefix = objectIdString[0];
        if (!PrefixToType.TryGetValue(prefix, out var type))
        {
            throw new ArgumentException($"Unrecognized prefix '{prefix}' in object ID.");
        }

        string uniqueId = objectIdString.Substring(1);
        return new ObjectIdentifier(type, uniqueId);
    }
}

public class DBTileValue
{
    public GridPosition Position { get; set; } // GridPosition für die Position des Tiles
    public GameObject TileObject { get; set; } // Referenz auf das GameObject des Tiles
    public TileType Type { get; set; } // Typ des Tiles
    public ResourceType Resource { get; set; } // Ressourcentyp des Tiles
    public List<GridPosition> AdjacentTilesPosition { get; set; } // Liste von angrenzenden Tile-Positionen
    public List<ObjectIdentifier> ConstructionClaims { get; set; } // Liste von Bauansprüchen
    public DBTileValue(GridPosition position, GameObject tileObject, TileType type, ResourceType resource)
    {
        Position = position;
        TileObject = tileObject;
        Type = type;
        Resource = resource;
        AdjacentTilesPosition = new List<GridPosition>();
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
    public int Columns { get; set; } // Number of tile columns
    public int Rows { get; set; } // Number of tile rows
    public SDBInitMapData(int columns, int rows){
        Columns = columns;
        Rows = rows;
    } 

    

}



