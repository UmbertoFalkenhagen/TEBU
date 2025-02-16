using System.Collections;
using System.Collections.Generic;
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
    // TODO: ObjectIdentifier are only for runtime DB, not for blueprints! They got other types, add SBInitMapData
    public Dictionary<BuildingType, SDBBuildingBlueprintValue> buildingBlueprintDictionary = new Dictionary<BuildingType, SDBBuildingBlueprintValue>();
    public Dictionary<AnimalType, SDBAnimalBlueprintValue> animalBlueprintDictionary = new Dictionary<AnimalType, SDBAnimalBlueprintValue>();
    public SDBInitMapData initMapData = new SDBInitMapData(11,11);
    //Singleton
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
    #region tileDictionary
    public void AddTile(GridPosition postition)
    {
        List<DBTileValue> tiles = new List<DBTileValue>();
        string uniqueId ="232333";
        ObjectIdentifier identifier = new ObjectIdentifier(ObjectType.Tile, uniqueId);

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
