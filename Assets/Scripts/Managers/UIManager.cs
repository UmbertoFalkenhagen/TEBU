using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance { get; private set; }
    private DatabaseManager databaseManager;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }

        Instance = this;
        DontDestroyOnLoad(gameObject);


    }
    // Start is called before the first frame update
    void Start()
    {
        databaseManager = DatabaseManager.Instance;
        if (databaseManager == null)
        {
            Debug.LogError("DatabaseManager could not be found!");
        }
        ActiveTile.OnActiveTileChanged += tileClick;

    }
    private void OnDestroy()
    {
        ActiveTile.OnActiveTileChanged -= tileClick;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void tileClick(HexTile newTile)
    {
        ObjectIdentifier tileID = newTile.TileID;
        //update resourceUI to tell player what resources are on it, happens even with building ontop

        if (databaseManager.TileDictionary.TryGetValue(tileID, out DBTileValue tileValue))  
            {
            GameObject hexTileObj = tileValue.TileObject;            

                if(newTile.heldBuilding != null) 
                { 
                ObjectIdentifier buildingID = newTile.GetHeldBuildingID();
                ObjectIdentifier cityID = newTile.GetHeldCityID();
                    if (buildingID == null && cityID != null)
                    {
                        //city ist hier logic
                        //1. update resourceBar based on
                        //                   ResourceBar.Instance.UpdateResourceBar(tileID, databaseManager);

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
                    Debug.Log("no Building here!!");

                    //logic wenn nichts gebaut wurde
                    //show build options
                    //show claims 
                    //show tile Informations in richt menu
                    //                    ScriptableBuilding sCC = Resources.Load<ScriptableBuilding>("")

                    ScriptableCityCenter sCC = Resources.Load<ScriptableCityCenter>("Data/Buildings/CityCenter_Grassland");
                    newTile.PlaceCityCenterOnTile(sCC);

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
