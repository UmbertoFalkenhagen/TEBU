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
        ObjectIdentifier structureID = databaseManager.GetStructureIdByTileId(activeTile.TileID);
        ProductDisplay.Instance.Clear();

        if (structureID != null)
        {
            BuildingPanel.Instance.buildingOptions.Clear();
            if (structureID.Type == ObjectType.CityCenter)
            {
                var cityId = structureID;

                //ressourceBar
                Dictionary<ProductType, int> productInventory = databaseManager.CityCenterDictionary[cityId]._inventory;


                //TODO: remove here this needs to be filled on create
                if (!productInventory.ContainsKey(ProductType.Bricks))
                {
                    productInventory[ProductType.Bricks] = 42; // Standardwert setzen
                    productInventory[ProductType.Logs] = 11;
                }
                ProductDisplay.Instance.UpdateResourceBar(productInventory);

                //                   ResourceBar.Instance.UpdateResourceBar(tileID, databaseManager);

                //show claims
                // show tile Infomation in right menu
            }
            else if (structureID.Type == ObjectType.Building)
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
