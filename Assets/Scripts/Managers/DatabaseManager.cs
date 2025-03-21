using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance;

    // Store all tile blueprint data here (public for easy assignment in Inspector)
    public List<ScriptableTile> tileBlueprints = new List<ScriptableTile>();

    //runtime databases for instanced objects

    //Tiles
    private Dictionary<ObjectIdentifier, DBTileValue> tileDictionary = new Dictionary<ObjectIdentifier, DBTileValue>();
    public IReadOnlyDictionary<ObjectIdentifier, DBTileValue> TileDictionary
        => tileDictionary;
    //CityCenters
    private Dictionary<ObjectIdentifier, DBCityCenterValue> cityCenterDictionary = new Dictionary<ObjectIdentifier, DBCityCenterValue>();
    public IReadOnlyDictionary<ObjectIdentifier, DBCityCenterValue> CityCenterDictionary
        => cityCenterDictionary;
    //Building
    private Dictionary<ObjectIdentifier, DBBuildingValue> buildingDictionary = new Dictionary<ObjectIdentifier, DBBuildingValue>();
    public IReadOnlyDictionary<ObjectIdentifier, DBBuildingValue> BuildingDictionary
        => buildingDictionary;
    //Animal
    private Dictionary<ObjectIdentifier, DBAnimalValue> animalDictionary = new Dictionary<ObjectIdentifier, DBAnimalValue>();
    public IReadOnlyDictionary<ObjectIdentifier, DBAnimalValue> AnimalDictionary 
        => animalDictionary;

    //static databases for persistent blueprints
    public Dictionary<BuildingType, SDBBuildingBlueprintValue> buildingBlueprintDictionary = new Dictionary<BuildingType, SDBBuildingBlueprintValue>();
    public Dictionary<AnimalType, SDBAnimalBlueprintValue> animalBlueprintDictionary = new Dictionary<AnimalType, SDBAnimalBlueprintValue>();
    public Dictionary<ResourceType, GameObject> resourceBlueprintDictionary = new Dictionary<ResourceType, GameObject>();
    //initMapData is initiated empty and gets filled in GameLoader
    public SDBInitMapData initMapData = new SDBInitMapData();
    //Singleton
    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    #region GeneralFunctions
    // --------------------------------------------------------------------
    // Generate a random unique ID for the given ObjectType
    // --------------------------------------------------------------------
    public ObjectIdentifier GenerateUniqueId(ObjectType type)
    {
        const int ID_LENGTH = 6; // Fixed length, e.g. 6 digits
        while (true)
        {
            // Generate a random integer in [0 .. 10^ID_LENGTH)
            int max = (int)Mathf.Pow(10, ID_LENGTH);
            int randomNumber = UnityEngine.Random.Range(0, max);

            // Format with leading zeros to ensure fixed length
            // e.g. "000123" for randomNumber=123
            string uniqueIdPart = randomNumber.ToString($"D{ID_LENGTH}");

            // Build the candidate ID
            ObjectIdentifier candidate = new ObjectIdentifier(type, uniqueIdPart);

            // Check if the candidate already exists
            bool exists = false;
            switch (type)
            {
                case ObjectType.Tile:
                    exists = tileDictionary.ContainsKey(candidate);
                    break;
                case ObjectType.CityCenter:
                    exists = cityCenterDictionary.ContainsKey(candidate);
                    break;
                case ObjectType.Building:
                    exists = buildingDictionary.ContainsKey(candidate);
                    break;
                case ObjectType.Animal:
                    exists = animalDictionary.ContainsKey(candidate);
                    break;
                default:
                    exists = true; // If we ever add more types without updating
                    break;
            }

            // If not found, return the fresh ID
            if (!exists)
                return candidate;

            // Otherwise, loop again and generate another random ID
        }
    }

    // --------------------------------------------------------------------
    // Find and return the data entry for an ObjectIdentifier
    // --------------------------------------------------------------------
    public object FindObjectByID(ObjectIdentifier identifier)
    {
        switch (identifier.Type)
        {
            case ObjectType.Tile:
                if (tileDictionary.TryGetValue(identifier, out var tileValue))
                    return tileValue; // DBTileValue
                break;

            case ObjectType.CityCenter:
                if (cityCenterDictionary.TryGetValue(identifier, out var cityCenterValue))
                    return cityCenterValue; // DBCityCenterValue
                break;

            case ObjectType.Building:
                if (buildingDictionary.TryGetValue(identifier, out var buildingValue))
                    return buildingValue; // DBBuildingValue
                break;

            case ObjectType.Animal:
                if (animalDictionary.TryGetValue(identifier, out var animalValue))
                    return animalValue; // DBAnimalValue
                break;
        }

        // If no matching entry found
        return null;
    }

    public ObjectIdentifier GetStructureIdByTileId(ObjectIdentifier tileId)
    {
        ObjectIdentifier structureId = GetCityCenterIdByTileId(tileId) ?? GetBuildingIdByTileId(tileId);
        return structureId;
    }
    #endregion

    #region tileDictionary
    public void AddTile(ObjectIdentifier tileID, DBTileValue tileValue)
    {
        if (tileDictionary.ContainsKey(tileID))
        {
            Debug.LogError($"DatabaseManager: Tile [{tileID}] already exists in tileDictionary! Count={tileDictionary.Count}");
            return;
        }

        tileDictionary[tileID] = tileValue;
        Debug.Log($"DatabaseManager: Added tile [{tileID}] to tileDictionary. Count={tileDictionary.Count}");
    }


    // 4) Query methods (for when HexTile wants data):
    //    Provide typed queries so HexTile can easily find info it needs.
    public DBTileValue GetTileValue(ObjectIdentifier tileID)
    {
        if (tileID.Type != ObjectType.Tile) return null;
        if (tileDictionary.TryGetValue(tileID, out var tileVal))
        {
            return tileVal;
        }
        return null;
    }

    // Maybe a typed resource accessor:
    public ResourceType GetTileResourceType(ObjectIdentifier tileID)
    {
        var val = GetTileValue(tileID);
        return val != null ? val.Resource : ResourceType.None;
    }

    // Return the actual tile GameObject from an ID (if you only want that):
    public GameObject GetTileGameObject(ObjectIdentifier tileID)
    {
        var val = GetTileValue(tileID);
        return val != null ? val.TileObject : null;
    }
    public ObjectIdentifier GetTileIDByPosition()
    {
        //TODO:
        return null;
    }

    // Return a list of neighbor DBTileValues
    public List<DBTileValue> GetTileNeighbors(ObjectIdentifier tileID)
    {
        var val = GetTileValue(tileID);
        if (val == null) return null;

        var neighbors = new List<DBTileValue>();
        foreach (var nPos in val.AdjacentTilesPosition)
        {
            // Attempt to look up by position
            var neighborVal = tileDictionary
                .Values
                .FirstOrDefault(t => t.Position == nPos);
            if (neighborVal != null) neighbors.Add(neighborVal);
        }
        return neighbors;
    }
    #endregion

    #region StaticBlueprintDBFunctions
    /// <summary>
    /// Populates the building blueprint dictionary with data from the provided ScriptableBuilding.
    /// </summary>
    public void AddBuildingBlueprint(ScriptableBuilding scriptableBuilding)
    {
        if (scriptableBuilding == null)
        {
            Debug.LogError("AddBuildingBlueprint: Provided ScriptableBuilding is null.");
            return;
        }

        // Create a new SDBBuildingBlueprintValue from the data
        SDBBuildingBlueprintValue blueprintValue = new SDBBuildingBlueprintValue
        {
            prefab = scriptableBuilding.basicPrefab,
            requiredTileTypes = new List<TileType>(scriptableBuilding.suitableTileTypeLocations),
            requiredResources = new List<ResourceType>(scriptableBuilding.requiredResources),
            outputProduct = scriptableBuilding.product,
            // If your ScriptableBuilding's inputProducts is a list of custom structs/classes,
            // you can translate them into a List<ProductType> or adapt as necessary:
            inputProducts = scriptableBuilding.inputProducts
                .Select(req => req.product)   // or req.theProductType, depending on your fields
                .ToList(),
            productionPerWorker = scriptableBuilding.productionPerWorker,
            isMaxWorkersFixed = scriptableBuilding.isMaxWorkersFixed,
            maxWorkers = scriptableBuilding.maxWorkers
        };

        // Store it in the dictionary, keyed by the building type
        buildingBlueprintDictionary[scriptableBuilding.buildingName] = blueprintValue;

        Debug.Log($"Building blueprint '{scriptableBuilding.buildingName}' added/updated in the dictionary.");
    }

    /// <summary>
    /// Populates the animal blueprint dictionary from the provided ScriptableAnimal.
    /// </summary>
    public void AddAnimalBlueprint(ScriptableAnimal scriptableAnimal)
    {
        if (scriptableAnimal == null)
        {
            Debug.LogError("AddAnimalBlueprint: Provided ScriptableAnimal is null.");
            return;
        }

        // Create a new SDBAnimalBlueprintValue from the data in ScriptableAnimal
        SDBAnimalBlueprintValue blueprintValue = new SDBAnimalBlueprintValue
        {
            prefab = scriptableAnimal.prefab,
            // Since 'spawnLocation' is a single TileType, we'll store it as a single-entry list.
            requiredTileTypes = new List<TileType> { scriptableAnimal.spawnLocation },

            basicFood = scriptableAnimal.basicFood,

            // Renaming to match the SDBAnimalBlueprintValue fields:
            ability1UnlockProduct = scriptableAnimal.basicAbilityUnlockProduct1,
            ability2UnlockProduct = scriptableAnimal.basicAbilityUnlockProduct2,
            ability1ImprovProduct = scriptableAnimal.abilityImprovementProduct1,
            ability2ImprovProduct = scriptableAnimal.abilityImprovementProduct2
        };

        // Key the dictionary by the AnimalType specified in ScriptableAnimal
        animalBlueprintDictionary[scriptableAnimal.animalName] = blueprintValue;

        Debug.Log($"Animal blueprint '{scriptableAnimal.animalName}' added/updated in the dictionary.");
    }

    public void AddResourceBlueprint(ScriptableResource scriptableResource)
    {
        resourceBlueprintDictionary[scriptableResource.resourcename] = scriptableResource.resourcePrefab;
    }

    public GameObject GetResourcePrefabForType(ResourceType resourceType)
    {
        if (resourceBlueprintDictionary.TryGetValue(resourceType, out GameObject prefab))
        {
            return prefab;
        }
        else
        {
            Debug.LogWarning($"ResourceType {resourceType} not found in the dictionary.");
            return null;
        }
    }


    #endregion

    #region buildingDictionary

    //AddBuilding()
    //RemoveBuilding()


    public ObjectIdentifier GetBuildingIdByTileId(ObjectIdentifier tileId)
    {
        foreach (var kvp in buildingDictionary)
        {
            if (kvp.Value._parentTile == tileId)
            {
                return kvp.Key; // Gibt die buildingId zurück
            }
        }
        return null; // Falls kein Gebäude gefunden wurde
    }
    public List<ObjectIdentifier> GetBuildingIdsByCityCenterId(ObjectIdentifier cityCenterId)
    {
        List<ObjectIdentifier> buildingIds = new List<ObjectIdentifier>();

        foreach (var kvp in buildingDictionary)
        {
            if (kvp.Value._parentCityCenter == cityCenterId)
            {
                buildingIds.Add(kvp.Key); // Fügt die passende buildingId zur Liste hinzu
            }
        }

        return buildingIds;
    }
    public ObjectIdentifier GetParentTileIdByBuildingId(ObjectIdentifier buildingId)
    {
        if (buildingDictionary.TryGetValue(buildingId, out DBBuildingValue buildingValue))
        {
            return buildingValue._parentTile; // Gibt die tileId des Gebäudes zurück
        }
        return null; // Falls die buildingId nicht existiert
    }
    #endregion

    #region cityCenterDictionary
    public void AddCityCenter(ObjectIdentifier cityCenterID, DBCityCenterValue cityCenterValue)
    {
        if (cityCenterDictionary.ContainsKey(cityCenterID))
        {
            Debug.LogError($"DatabaseManager: City Center [{cityCenterID}] already exists in cityCenterDictionary! Count={cityCenterDictionary.Count}");
            return;
        }

        cityCenterDictionary[cityCenterID] = cityCenterValue;
        Debug.Log($"DatabaseManager: Added City Center [{cityCenterID}] to cityCenterDictionary. Count={cityCenterDictionary.Count}");
    }

    public void RemoveCityCenter(ObjectIdentifier cityCenterID)
    {
        if (!cityCenterDictionary.ContainsKey(cityCenterID))
        {
            Debug.LogError($"DatabaseManager: City Center [{cityCenterID}] does not exist in cityCenterDictionary! Count={cityCenterDictionary.Count}");
            return;
        }

        cityCenterDictionary.Remove(cityCenterID);
        Debug.Log($"DatabaseManager: Removed City Center [{cityCenterID}] from cityCenterDictionary. Count={cityCenterDictionary.Count}");
    }

    public ObjectIdentifier GetCityCenterIdByTileId(ObjectIdentifier tileId)
    {
        foreach (var kvp in cityCenterDictionary)
        {
            if (kvp.Value._parentTile == tileId)
            {
                return kvp.Key; // Gibt die buildingId zurück
            }
        }
        return null; // Falls kein Gebäude gefunden wurde
    }

    public ObjectIdentifier GetParentTileIdByCityCenterId(ObjectIdentifier cityCenterId)
    {
        if (cityCenterDictionary.TryGetValue(cityCenterId, out DBCityCenterValue cityCenterValue))
        {
            return cityCenterValue._parentTile; // Gibt die tileId des Gebäudes zurück
        }
        return null; // Falls die buildingId nicht existiert
    }
    #endregion
}
