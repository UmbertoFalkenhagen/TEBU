using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region Singleton & References

    public static UIManager Instance { get; private set; }
    private DatabaseManager databaseManager;

    #endregion

    #region Configuration

    [Header("UI System Selection")]
    [Tooltip("If true, uses new UI Toolkit system. If false, uses legacy Canvas UI")]
    public bool useNewUISystem = true;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
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

        ActiveTile.OnActiveTileChanged += TileClick;
    }

    private void OnDestroy()
    {
        ActiveTile.OnActiveTileChanged -= TileClick;
    }

    #endregion

    #region Tile Click Handler

    public void TileClick(ObjectIdentifier activeTileID)
    {
        DBTileValue tileValue = databaseManager.GetTileValue(activeTileID);
        ObjectIdentifier structureID = databaseManager.GetStructureId(activeTileID);

        ProductDisplay.Instance.Clear();

        if (useNewUISystem)
        {
            if (AnimalManagementUI.Instance != null)
            {
                AnimalManagementUI.Instance.ShowTileUI(activeTileID);
                ShowBuildingAndProductUI(activeTileID, tileValue, structureID);
            }
            else
            {
                Debug.LogWarning("AnimalManagementUI instance not found. Falling back to legacy UI.");
                ShowLegacyUI(activeTileID, tileValue, structureID);
            }
            return;
        }

        ShowLegacyUI(activeTileID, tileValue, structureID);
    }

    #endregion

    #region New UI System Support

    private void ShowBuildingAndProductUI(ObjectIdentifier activeTileID, DBTileValue tileValue, ObjectIdentifier structureID)
    {
        if (structureID != null)
        {
            BuildingPanel.Instance.buildingOptions.Clear();

            if (structureID.Type == ObjectType.CityCenter)
            {
                var cityId = structureID;
                Dictionary<ProductType, int> productInventory = databaseManager.CityCenterDictionary[cityId]._inventory;

                if (!productInventory.ContainsKey(ProductType.Bricks))
                {
                    productInventory[ProductType.Bricks] = 42 + databaseManager.CityCenterDictionary.Count;
                    productInventory[ProductType.Logs] = 12;
                }
                ProductDisplay.Instance.UpdateResourceBar(productInventory);
            }
            else if (structureID.Type == ObjectType.Building)
            {
                ObjectIdentifier parentCityId = databaseManager.GetCityCenterIdByBuildingId(structureID);
                Dictionary<ProductType, int> productInventory = databaseManager.CityCenterDictionary[parentCityId]._inventory;
                ProductDisplay.Instance.UpdateResourceBar(productInventory);
            }

            BuildingPanel.Instance.UpdateBuildingOptions();
        }
        else
        {
            if (tileValue.ConstructionClaims.Count == 0 || databaseManager.GetCityCenterIdByTileId(activeTileID) != null)
            {
                BuildingPanel.Instance.buildingOptions.Clear();
                BuildingPanel.Instance.buildingOptions.Add("CityCenter", "Build City Center");
                BuildingPanel.Instance.UpdateBuildingOptions();
            }
            else
            {
                PopulateBuildingOptions(activeTileID, tileValue);
            }
        }
    }

    #endregion

    #region Legacy UI System

    private void ShowLegacyUI(ObjectIdentifier activeTileID, DBTileValue tileValue, ObjectIdentifier structureID)
    {
        InfoPanel.Instance.DisplayStuff(tileValue);

        if (structureID != null)
        {
            BuildingPanel.Instance.buildingOptions.Clear();

            if (structureID.Type == ObjectType.CityCenter)
            {
                var cityId = structureID;
                Dictionary<ProductType, int> productInventory = databaseManager.CityCenterDictionary[cityId]._inventory;

                if (!productInventory.ContainsKey(ProductType.Bricks))
                {
                    productInventory[ProductType.Bricks] = 42 + databaseManager.CityCenterDictionary.Count;
                    productInventory[ProductType.Logs] = 12;
                }
                ProductDisplay.Instance.UpdateResourceBar(productInventory);
            }
            else if (structureID.Type == ObjectType.Building)
            {
                ObjectIdentifier parentCityId = databaseManager.GetCityCenterIdByBuildingId(structureID);
                Dictionary<ProductType, int> productInventory = databaseManager.CityCenterDictionary[parentCityId]._inventory;
                ProductDisplay.Instance.UpdateResourceBar(productInventory);
            }

            BuildingPanel.Instance.UpdateBuildingOptions();
        }
        else
        {
            if (tileValue.ConstructionClaims.Count == 0 || databaseManager.GetCityCenterIdByTileId(activeTileID) != null)
            {
                BuildingPanel.Instance.buildingOptions.Clear();
                BuildingPanel.Instance.buildingOptions.Add("CityCenter", "Build City Center");
                BuildingPanel.Instance.UpdateBuildingOptions();
            }
            else
            {
                PopulateBuildingOptions(activeTileID, tileValue);
            }
        }
    }

    #endregion

    #region Building Options Logic

    private void PopulateBuildingOptions(ObjectIdentifier activeTileID, DBTileValue tileValue)
    {
        BuildingPanel.Instance.buildingOptions.Clear();

        if (tileValue == null)
        {
            Debug.LogError($"No tile value found for ID {activeTileID}");
            return;
        }

        TileType tileType = tileValue.Type;
        ResourceType tileResource = tileValue.Resource;

        List<BuildingType> suitableBuildings = new List<BuildingType>();

        foreach (var kvp in databaseManager.buildingBlueprintDictionary)
        {
            BuildingType buildingType = kvp.Key;
            SDBBuildingBlueprintValue blueprint = kvp.Value;

            if (!blueprint.requiredTileTypes.Contains(tileType))
                continue;

            bool isResourceRequirementMet = false;

            if (blueprint.requiredResources.Contains(ResourceType.None))
            {
                isResourceRequirementMet = true;
            }
            else
            {
                if (blueprint.requiredResources.Contains(tileResource))
                {
                    isResourceRequirementMet = true;
                }
            }

            if (isResourceRequirementMet)
            {
                BuildingPanel.Instance.buildingOptions.Add(
                    buildingType.ToString(),
                    $"Build {buildingType}"
                );

                suitableBuildings.Add(buildingType);
            }
        }

        if (suitableBuildings.Count > 0)
        {
            string debugList = string.Join(", ", suitableBuildings);
            Debug.Log($"Suitable buildings for tile [{activeTileID}]: {debugList}");
        }
        else
        {
            Debug.Log($"No suitable buildings found for tile [{activeTileID}].");
        }

        BuildingPanel.Instance.UpdateBuildingOptions();
    }

    #endregion

    #region Build Request Handler

    public void RequestBuild(string buildingType)
    {
        if (buildingType == "CityCenter")
        {
            CityCenterManager.Instance.Build();
        }
        else
        {
            Debug.Log("Trying to build a " + buildingType);
            BuildingManager.Instance.Build(buildingType);
        }
    }

    #endregion
}
