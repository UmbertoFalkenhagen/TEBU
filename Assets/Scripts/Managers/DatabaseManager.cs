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
    public SDBInitMapData initMapData = new();
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
        /* This is a test to see if creating IDs works
        // 1) Generate a couple of unique IDs to see them in the console
        ObjectIdentifier tileId1 = GenerateUniqueId(ObjectType.Tile);
        ObjectIdentifier tileId2 = GenerateUniqueId(ObjectType.Tile);
        Debug.Log($"Generated Tile IDs: {tileId1}, {tileId2}");

        // 2) Add two Tiles to the dictionary (internally calls GenerateUniqueId)
        AddTile(new GridPosition(0, 0));  // Creates & logs a Tile entry
        AddTile(new GridPosition(1, 1));  // Creates & logs another Tile entry

        // 3) Let's pick the first tile ID from the dictionary
        var firstTileId = tileDictionary.Keys.First();
        Debug.Log($"First tile ID in dictionary: {firstTileId}");

        // 4) Try to find that tile in the database
        var foundData = FindObjectByID(firstTileId);
        if (foundData != null)
        {
            // foundData should be a DBTileValue in this case
            Debug.Log($"Found tile data for ID {firstTileId}: {foundData}");
        }
        else
        {
            Debug.LogWarning($"No tile found with ID: {firstTileId}");
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region GeneralFunctions
    // --------------------------------------------------------------------
    // 1) Generate a random unique ID for the given ObjectType
    //    Uses a FIXED ID length (e.g., 6 digits)
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
    // 2) Find and return the data entry for an ObjectIdentifier
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
    public void AddTile(GridPosition postition)
    {
        List<DBTileValue> tiles = new List<DBTileValue>();
        ObjectIdentifier identifier = GenerateUniqueId(ObjectType.Tile);

        if (!tileDictionary.ContainsKey(identifier))
        {

            // Erstelle eine GridPosition für das Tile
            GridPosition tilePosition = postition; // Beispielposition (x=0, y=0)
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

}
