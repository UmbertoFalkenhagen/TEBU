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
    public Dictionary<ObjectIdentifier, SDBBuildingBlueprintValue> buildingBlueprintDictionary = new Dictionary<ObjectIdentifier, SDBBuildingBlueprintValue>();
    public Dictionary<ObjectIdentifier, SDBAnimalBlueprintValue> animalBlueprintDictionary = new Dictionary<ObjectIdentifier, SDBAnimalBlueprintValue>();

    //Singleton
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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
