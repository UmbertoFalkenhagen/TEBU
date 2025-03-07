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
        if (activeTile.heldBuilding != null)
        {
            ObjectIdentifier buildingID = activeTile.GetHeldBuildingID();
            ObjectIdentifier cityID = activeTile.GetHeldCityID();
            BuildingPanel.Instance.buildingOptions.Clear();
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
            BuildingPanel.Instance.UpdateBuildingOptions();

        }
        else
        {
            //logic wenn tile leer ist
            if (activeTile.constructionClaims.Count == 0)
            {
                BuildingPanel.Instance.buildingOptions.Clear();
                BuildingPanel.Instance.buildingOptions.Add("CityCenter", "Build City Center");
                //TODO: add only CityCenter build button...
                BuildingPanel.Instance.UpdateBuildingOptions();  //show build options

            }
            else
            {
                BuildingPanel.Instance.buildingOptions.Clear();
                //TODO: there are claims, add bulding options based on them
            }
            //show claims 
            //show tile Informations in rechten menu
        }
    }
    public void RequestBuild(string buildingType)
    {
        if(buildingType == "CityCenter")
        {
            //TODO: Add check if enough resources
            CityCenterManager.Instance.Build();
        }
        else
        {
            //TODO: Add check if enough resources
           // BuildingManager.Instance.Build(buildingType);
        }


        //callBuildingManager

        //if cityCenter

    }


}
