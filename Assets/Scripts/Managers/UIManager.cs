using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance;
    private DatabaseManager databaseManager;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

    }
    // Start is called before the first frame update
    void Start()
    {
        databaseManager = DatabaseManager.Instance;
        if (databaseManager == null)
        {
            Debug.LogError("DatabaseManager could not be found!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void tileClick(ObjectIdentifier tileID)
    {
        //update resourceUI to tell player what resources are on it, happens even with building ontop

        if (databaseManager.TileDictionary.TryGetValue(tileID, out DBTileValue tileValue))  
            {
            GameObject hexTile = tileValue.TileObject;
            if (hexTile.TryGetComponent(out HexTile hexTileScript))
            {
                if (hexTileScript.heldBuilding != null)
                {
                    Debug.Log(hexTileScript.heldBuilding.ToString());
                    //  Debug.Log(hexTileScript.heldBuilding.GetComponent<>)

                    //check if held building is citycenter or building
                    //if citycenter get ID and get resources with it

                    //if building get parent citycenter id, get recources

                    // -> click -> check if hexTIleComponent ->get HexTileComponent ->
                    // Get Tile ID -> get tileValue -> get TileObject -> get hexTileScript -> get held building -> 
                    // get buildingComponnent -> get building id -> if city get recources/if building get praentcity id -> get resources

                }
                else
                {
                    Debug.Log("No held Building in this tile");
                }
            }
            else
            {
                Debug.LogError ("HexTile Script not found!");
            }


            // Debug.Log("value "); 
        } 
        /*elseif(hold building = citycenter)
        show citycenter information with database tileid
        elseif(hold building = building)
        show building infomation based on database tileid

        */
        //check if building or city is on it
        //update uI based on those

        //code to update based on tile information;
    }
}
