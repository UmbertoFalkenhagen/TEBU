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

    public void tileClick(ObjectIdentifier activeTileID)
    {
        DBTileValue tileValue = databaseManager.GetTileValue(activeTileID);
        ObjectIdentifier structureID = databaseManager.GetStructureId(activeTileID);
        ProductDisplay.Instance.Clear();

        // sollte das wirklich über den ID check gehen, oder sollte das über check von active claim/construction claim gehen??
        // aka: if(active CLaim == vorhanden) if(activeClaim.Type == CityCenter) ....elseif(activeClaim == null) -> check construction Claims and show resources of top construction claim
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
                    productInventory[ProductType.Bricks] = 42 + databaseManager.CityCenterDictionary.Count; // Standardwert setzen
                    productInventory[ProductType.Logs] = 12;
                }
                ProductDisplay.Instance.UpdateResourceBar(productInventory);

                //                   ResourceBar.Instance.UpdateResourceBar(tileID, databaseManager);

                //show claims
                // show tile Infomation in right menu
            }
            else if (structureID.Type == ObjectType.Building)
            {
                //Update Product Display, based on this buildings CityCenter
                Dictionary<ProductType, int> productInventory = databaseManager.CityCenterDictionary[databaseManager.BuildingDictionary[structureID]._parentCityCenter]._inventory;
                ProductDisplay.Instance.UpdateResourceBar(productInventory);

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
            
            if (tileValue.ConstructionClaims.Count == 0 || databaseManager.GetCityCenterIdByTileId(activeTileID) != null)
            {
                
                BuildingPanel.Instance.buildingOptions.Clear();
                BuildingPanel.Instance.buildingOptions.Add("CityCenter", "Build City Center");
                //TODO: add only CityCenter build button...
                BuildingPanel.Instance.UpdateBuildingOptions();  //show build options

            }
            else
            {
                BuildingPanel.Instance.buildingOptions.Clear();
                Debug.Log("Hello");

                if (tileValue == null)
                {
                    Debug.LogError($"No tile value found for ID {activeTileID}");
                    return;
                }

                TileType tileType = tileValue.Type;
                ResourceType tileResource = tileValue.Resource;

                // We'll collect all suitable building types for debugging
                List<BuildingType> suitableBuildings = new List<BuildingType>();

                foreach (var kvp in databaseManager.buildingBlueprintDictionary)
                {
                    BuildingType buildingType = kvp.Key;
                    SDBBuildingBlueprintValue blueprint = kvp.Value;

                    // 1) Tile Type check
                    if (!blueprint.requiredTileTypes.Contains(tileType))
                        continue;

                    // 2) Resource requirement logic
                    bool isResourceRequirementMet = false;
                    
                        // If ResourceType.None is present, that means "no resource restriction"
                        if (blueprint.requiredResources.Contains(ResourceType.None))
                        {
                            isResourceRequirementMet = true;
                        }
                        else
                        {
                            // Must match one of the explicitly listed resources
                            if (blueprint.requiredResources.Contains(tileResource))
                            {
                                isResourceRequirementMet = true;
                            }
                        }


                    // If both tile type and resource requirements are satisfied, add an option
                    if (isResourceRequirementMet)
                    {
                        // Add to building panel
                        BuildingPanel.Instance.buildingOptions.Add(
                            buildingType.ToString(),
                            $"Build {buildingType}"
                        );

                        // Also collect the building type for debug logging
                        suitableBuildings.Add(buildingType);
                    }
                }

                // After we finish determining suitable buildings, log them for debugging
                if (suitableBuildings.Count > 0)
                {
                    string debugList = string.Join(", ", suitableBuildings);
                    Debug.Log($"Suitable buildings for tile [{activeTileID}]: {debugList}");
                }
                else
                {
                    Debug.Log($"No suitable buildings found for tile [{activeTileID}].");
                }

                // Finally, update the UI
                BuildingPanel.Instance.UpdateBuildingOptions();
            }

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
            Debug.Log("Trying to build a " + buildingType);
           BuildingManager.Instance.Build(buildingType);
        }


        //callBuildingManager

        //if cityCenter

    }



}
