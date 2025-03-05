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

    public void tileClick(HexTile activeTile)
    {
        Debug.Log("Tile Active:" + ActiveTile.Instance.GetActiveTileID().ToString());
        if (activeTile.heldBuilding != null)
        {
            ObjectIdentifier buildingID = activeTile.GetHeldBuildingID();
            ObjectIdentifier cityID = activeTile.GetHeldCityID();
            if (buildingID == null && cityID != null)
            {
                //city ist hier logic
                //1. update resourceBar based on
                //                   ResourceBar.Instance.UpdateResourceBar(tileID, databaseManager);

                //show claims
                // show tile Infomation in right menu
            }
            else if (buildingID != null && cityID == null)
            {
                //building ist hier logic


                //Update Building Panel

                //get parent city center and update resource bar based on it
                //  resourceScript.UpdateResourceBar(tileID, databaseManager);

                //show claims
                //show tile Information in right menu
            }
        }
        else
        {
            //logic wenn tile leer ist
            BuildingPanel.Instance.UpdateBuildingOptions(activeTile);  //show build options
                                                              //show claims 
                                                              //show tile Informations in rechten menu
        }
    }
    public void RequestBuild()
    {

        //if() decied if city or building and call
        //callBuildingManager

        //if cityCenter
        CityCenterManager.Instance.Build();

    }


}
