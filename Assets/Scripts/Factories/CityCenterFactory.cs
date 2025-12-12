using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CityCenterFactory : MonoBehaviour
{
    public static CityCenterFactory Instance;

    List<string> cityNames = new List<string>
{
    "Wolfspire",
    "Bearhollow",
    "Lynxtide",
    "Vulture's Roost",
    "Hawkstone",
    "Ravenbrook",
    "Staghelm",
    "Ocelridge",
    "Boarcliff",
    "Cobrathorn",
    "Foxden",
    "Eaglewatch",
    "Panther's Reach",
    "Sablegate",
    "Turtlefall",
    "Bisonstead",
    "Badgerhold",
    "Serpent's Crossing",
    "Ottermire",
    "Crowhurst",
    "Frogshadow",
    "Koiport",
    "Beetleford",
    "Lizardfen",
    "Gryphonsrest",
    "Mantisfield",
    "Jackalbay",
    "Owlshade",
    "Caribou Creek",
    "Waspnest",
    "Puma's End",
    "Goosemarsh",
    "Weaselwick",
    "Stallion's Hollow",
    "Shrikehaven",
    "Salmonreach",
    "Bullhorn",
    "Cougarstead",
    "Finchmeadow",
    "Crabhaven",
    "Peacock's Bluff",
    "Vixenhold",
    "Hyenavale",
    "Buffalo Heights",
    "Toadgrove",
    "Rookpoint",
    "Mustang's Rest",
    "Spiderroot",
    "Pelican Shore",
    "Molehill",
    "Scorpiongate",
    "Falcon's Rise",
    "Lobstercliff",
    "Hareburrow",
    "Wyrmfrost",
    "Eelwater",
    "Kite's Hollow",
    "Ramhorn",
    "Parrotport",
    "Houndridge",
    "Beetlebarrow",
    "Walrusbay",
    "Swanfrost",
    "Chameleon Creek",
    "Ferretford",
    "Heronspire",
    "Batshade",
    "Moosegrove",
    "Antlerstead",
    "Cobraquay",
    "Coyote's Howl",
    "Magpiefield",
    "Gazellecross",
    "Turtleback",
    "Crowspire",
    "Dragonfly Cove",
    "Jackalope Haven",
    "Viper's Nest",
    "Oxpoint",
    "Starlingmoor",
    "Lynxgate",
    "Orcahaven",
    "Terncliff",
    "Puma Ridge",
    "Frogfen",
    "Spiderfell",
    "Whalebreach",
    "Eagle's Crest",
    "Larkholm",
    "Squirrel’s Rest",
    "Piranha Waters",
    "Hawk’s Landing",
    "Wolfpine",
    "Beartrap Vale",
    "Ratborough",
    "Crayfish Hollow",
    "Hornet’s Reach",
    "Vulturespine",
    "Beaverbrook",
    "Cheetahwind"
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

    public KeyValuePair<ObjectIdentifier, DBCityCenterValue> CreateCityCenter(ScriptableCityCenter cityCenterData, HexTile parentTile)
    {
        Vector3 worldPosition = parentTile.gameObject.transform.position;
        Transform parent = parentTile.gameObject.transform;
        if (cityCenterData == null)
        {
            Debug.LogError("CityCenterFactory: citycenterdata is null!");
            return default;
        }

        //clear resource from tile
        parentTile.ClearTileResource();

        //generate new objectidentifier for the citycenter
        ObjectIdentifier cityCenterID = DatabaseManager.Instance.GenerateUniqueId(ObjectType.CityCenter);

        //instantiate citycenterprefab on the tile
        GameObject cityCenterObject = Instantiate(cityCenterData.basicPrefab, worldPosition, cityCenterData.basicPrefab.transform.rotation, parent);
        if (cityCenterObject == null)
        {
            Debug.LogError("CityCenterFactory: Failed to instantiate citycenter prefab.");
            return default;
        }
        parentTile.heldBuilding = cityCenterObject;

        //ensure city center component on the gameobject
        CityCenter cityCenterComponent = cityCenterObject.GetComponent<CityCenter>();
        if (cityCenterComponent == null)
        {
            cityCenterComponent = cityCenterObject.AddComponent<CityCenter>();
        }

        string cityName = GetAndRemoveRandomCityName();
        Dictionary<ProductType, int> productInventory = new Dictionary<ProductType, int>();
        DBCityCenterValue dBCityCenterValue = new DBCityCenterValue(parentTile.TileID, cityCenterData.basicPrefab, productInventory, 6, cityName);
        cityCenterComponent.cityCenterID = cityCenterID;

        return new KeyValuePair<ObjectIdentifier, DBCityCenterValue>(cityCenterID, dBCityCenterValue);
    }

    public string GetAndRemoveRandomCityName()
    {
        if (cityNames.Count == 0)
        {
            Debug.LogError("City name list is empty! No names left to assign.");
            return "Unnamed City";
        }

        int randomIndex = UnityEngine.Random.Range(0, cityNames.Count);
        string selectedName = cityNames[randomIndex];
        cityNames.RemoveAt(randomIndex);

        Debug.Log($"Assigned city name: {selectedName} | Remaining names: {cityNames.Count}");
        return selectedName;
    }

}
