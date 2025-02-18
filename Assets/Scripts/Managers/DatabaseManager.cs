using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance;

    //runtime databases for instanced objects
    public Dictionary<ObjectIdentifier, DBTileValue> tileDictionary = new Dictionary<ObjectIdentifier, DBTileValue>();
    public Dictionary<ObjectIdentifier, DBCityCenterValue> cityCenterDictionary = new Dictionary<ObjectIdentifier, DBCityCenterValue>();
    public Dictionary<ObjectIdentifier, DBBuildingValue> buildingDictionary = new Dictionary<ObjectIdentifier, DBBuildingValue>();
    public Dictionary<ObjectIdentifier, DBAnimalValue> animalDictionary = new Dictionary<ObjectIdentifier, DBAnimalValue>();

    //static databases for persistent blueprints
    public Dictionary<BuildingType, SDBBuildingBlueprintValue> buildingBlueprintDictionary = new Dictionary<BuildingType, SDBBuildingBlueprintValue>();
    public Dictionary<AnimalType, SDBAnimalBlueprintValue> animalBlueprintDictionary = new Dictionary<AnimalType, SDBAnimalBlueprintValue>();
    //initMapData is initiated empty and gets filled in GameLoader
    public SDBInitMapData initMapData = new SDBInitMapData();
    //Singleton
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //where does it come from? do we need it? 
           // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
     }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

    #endregion

    #region tileDictionary
    public void AddTile(Vector2Int postition)
    {
        List<DBTileValue> tiles = new List<DBTileValue>();
        ObjectIdentifier identifier = GenerateUniqueId(ObjectType.Tile);

        if (!tileDictionary.ContainsKey(identifier))
        {

            // Erstelle eine GridPosition für das Tile
            Vector2Int tilePosition = postition; // Beispielposition (x=0, y=0)
            // Erstelle ein GameObject für das Tile (dies sollte in der tatsächlichen Implementierung ein echtes GameObject sein)
            GameObject tileObject = new GameObject("TileObject");
            // Erstelle ein DBTileValue mit den entsprechenden Werten
            DBTileValue tileValue = new DBTileValue(tilePosition, tileObject, TileType.Grassland, ResourceType.Rice);



            // Füge die Liste mit dem einzelnen DBTileValue dem Dictionary hinzu
            tileDictionary[identifier] = tileValue;

            // tileDictionary[identifier] = null; // Füge die Tiles zur Liste hinzu
           // tileDictionary[identifier] = singleTileValue; // Neue Liste erstellen, wenn der Key nicht existiert
        }
        Debug.Log($"Tiles für {identifier} hinzugefügt. Gesamtanzahl: {tileDictionary.Count}");
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


    #endregion
}
