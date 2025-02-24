using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance;
    private DatabaseManager databaseManager;
    private static ResourceBar resourceScript;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (resourceScript == null) resourceScript = FindObjectOfType<ResourceBar>();
        else Debug.Log("ResourceBar not found");

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
                if(hexTileScript.heldBuilding != null) { 
                ObjectIdentifier buildingID = hexTileScript.GetHeldBuildingID();
                ObjectIdentifier cityID = hexTileScript.GetHeldCityID();
                    if (buildingID == null && cityID != null)
                    {
                        //city ist hier logic
                        //1. update resourceBar based on
                        //                    resourceScript.UpdateResourceBar(tileID, databaseManager);

                        //show claims
                        // show tile Infomation in right menu
                    }
                    else if(buildingID != null && cityID == null)
                    {
                        //building ist hier logic
                        Debug.Log("BuildingID: " + buildingID);
                        //get parent city center and update resource bar based on it
                      //  resourceScript.UpdateResourceBar(tileID, databaseManager);

                        //show claims
                        //show tile Information in right menu
                    }
                }
                else
                {
                    //logic wenn nichts gebaut wurde
                    //show build options
                    //show claims 
                    //show tile Informations in richt menu
                    Debug.Log("no Building here!!");
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
