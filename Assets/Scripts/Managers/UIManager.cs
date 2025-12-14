using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

/// <summary>
/// Central manager for handling tile-based UI interactions.
/// Routes tile click events to either the new UI Toolkit system or legacy Canvas UI.
/// Manages building option display and construction requests.
/// </summary>
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
        // Singleton pattern setup
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
        // Cache DatabaseManager reference
        databaseManager = DatabaseManager.Instance;
        if (databaseManager == null)
        {
            Debug.LogError("DatabaseManager could not be found!");
        }

        // Subscribe to tile click events from ActiveTile system
        // Flow: ClickManager ? ActiveTile.SetActiveTile ? OnActiveTileChanged event ? tileClick()
        ActiveTile.OnActiveTileChanged += tileClick;
    }

    private void OnDestroy()
    {
        // Clean up event subscription
        ActiveTile.OnActiveTileChanged -= tileClick;
    }

    #endregion

    #region Tile Click Handler

    /// <summary>
    /// Main entry point for tile click events.
    /// Routes to new UI Toolkit system or legacy Canvas UI based on Inspector toggle.
    /// Always clears product display before showing new UI.
    /// </summary>
    /// <param name="activeTileID">The ObjectIdentifier of the clicked tile</param>
    public void tileClick(ObjectIdentifier activeTileID)
    {
        // Fetch tile data and structure (if present)
        DBTileValue tileValue = databaseManager.GetTileValue(activeTileID);
        ObjectIdentifier structureID = databaseManager.GetStructureId(activeTileID);

        // Clear any existing product display
        ProductDisplay.Instance.Clear();

        // === NEW UI TOOLKIT SYSTEM ===
        // Shows unified tile info + management interface
        if (useNewUISystem)
        {
            if (AnimalManagementUI.Instance != null)
            {
                AnimalManagementUI.Instance.ShowTileUI(activeTileID);
            }
            else
            {
                // Fallback to legacy if new UI isn't available
                Debug.LogWarning("AnimalManagementUI instance not found. Falling back to legacy UI.");
                ShowLegacyUI(activeTileID, tileValue, structureID);
            }
            return;
        }

        // === LEGACY CANVAS UI SYSTEM ===
        // Original behavior with InfoPanel and BuildingPanel
        ShowLegacyUI(activeTileID, tileValue, structureID);
    }

    #endregion

    #region Legacy UI System

    /// <summary>
    /// Displays the original Canvas-based UI panels (InfoPanel and BuildingPanel).
    /// Handles three scenarios:
    /// 1. Tile has structure ? Show structure info and inventory
    /// 2. Tile is empty but claimable ? Show buildable options
    /// 3. Tile is unclaimed ? Show city center build option
    /// </summary>
    private void ShowLegacyUI(ObjectIdentifier activeTileID, DBTileValue tileValue, ObjectIdentifier structureID)
    {
        // Always show tile information panel
        InfoPanel.Instance.DisplayStuff(tileValue);

        // === STRUCTURE EXISTS ON TILE ===
        if (structureID != null)
        {
            BuildingPanel.Instance.buildingOptions.Clear();

            if (structureID.Type == ObjectType.CityCenter)
            {
                // Display city center inventory (resources like Bricks, Logs)
                var cityId = structureID;
                Dictionary<ProductType, int> productInventory = databaseManager.CityCenterDictionary[cityId]._inventory;

                // Initialize with default values if inventory is empty
                if (!productInventory.ContainsKey(ProductType.Bricks))
                {
                    productInventory[ProductType.Bricks] = 42 + databaseManager.CityCenterDictionary.Count;
                    productInventory[ProductType.Logs] = 12;
                }
                ProductDisplay.Instance.UpdateResourceBar(productInventory);
            }
            else if (structureID.Type == ObjectType.Building)
            {
                // Display parent city's inventory for buildings
                ObjectIdentifier parentCityId = databaseManager.GetCityCenterIdByBuildingId(structureID);
                Dictionary<ProductType, int> productInventory = databaseManager.CityCenterDictionary[parentCityId]._inventory;
                ProductDisplay.Instance.UpdateResourceBar(productInventory);
            }

            // Show structure-specific building options
            BuildingPanel.Instance.UpdateBuildingOptions();
        }
        // === TILE IS EMPTY ===
        else
        {
            // Check if tile can have a city center built
            // Allowed if: no construction claims OR already has city center claim
            if (tileValue.ConstructionClaims.Count == 0 || databaseManager.GetCityCenterIdByTileId(activeTileID) != null)
            {
                // Show city center build option
                BuildingPanel.Instance.buildingOptions.Clear();
                BuildingPanel.Instance.buildingOptions.Add("CityCenter", "Build City Center");
                BuildingPanel.Instance.UpdateBuildingOptions();
            }
            else
            {
                // Tile is claimed by a city - show buildable buildings based on tile type and resource
                PopulateBuildingOptions(activeTileID, tileValue);
            }
        }
    }

    #endregion

    #region Building Options Logic

    /// <summary>
    /// Populates the building options panel with structures that can be built on this tile.
    /// Filters blueprints based on:
    /// 1. Tile type compatibility (e.g., Forest, Mountain, Grassland)
    /// 2. Resource requirements (e.g., requires Wood resource)
    /// </summary>
    private void PopulateBuildingOptions(ObjectIdentifier activeTileID, DBTileValue tileValue)
    {
        BuildingPanel.Instance.buildingOptions.Clear();

        if (tileValue == null)
        {
            Debug.LogError($"No tile value found for ID {activeTileID}");
            return;
        }

        // Get tile properties for filtering
        TileType tileType = tileValue.Type;
        ResourceType tileResource = tileValue.Resource;

        List<BuildingType> suitableBuildings = new List<BuildingType>();

        // Iterate through all building blueprints
        foreach (var kvp in databaseManager.buildingBlueprintDictionary)
        {
            BuildingType buildingType = kvp.Key;
            SDBBuildingBlueprintValue blueprint = kvp.Value;

            // === CHECK 1: TILE TYPE ===
            // Building must support this tile type (e.g., Lumberjack requires Forest)
            if (!blueprint.requiredTileTypes.Contains(tileType))
                continue;

            // === CHECK 2: RESOURCE REQUIREMENT ===
            bool isResourceRequirementMet = false;

            // If blueprint accepts any resource (ResourceType.None)
            if (blueprint.requiredResources.Contains(ResourceType.None))
            {
                isResourceRequirementMet = true;
            }
            else
            {
                // Check if tile's resource matches blueprint requirements
                if (blueprint.requiredResources.Contains(tileResource))
                {
                    isResourceRequirementMet = true;
                }
            }

            // === ADD TO OPTIONS ===
            if (isResourceRequirementMet)
            {
                BuildingPanel.Instance.buildingOptions.Add(
                    buildingType.ToString(),
                    $"Build {buildingType}"
                );

                suitableBuildings.Add(buildingType);
            }
        }

        // Log results for debugging
        if (suitableBuildings.Count > 0)
        {
            string debugList = string.Join(", ", suitableBuildings);
            Debug.Log($"Suitable buildings for tile [{activeTileID}]: {debugList}");
        }
        else
        {
            Debug.Log($"No suitable buildings found for tile [{activeTileID}].");
        }

        // Update UI with filtered options
        BuildingPanel.Instance.UpdateBuildingOptions();
    }

    #endregion

    #region Build Request Handler

    /// <summary>
    /// Handles build requests from UI (legacy BuildingPanel).
    /// Routes to appropriate manager based on structure type.
    /// </summary>
    /// <param name="buildingType">The type of structure to build (e.g., "CityCenter", "Lumberjack")</param>
    public void RequestBuild(string buildingType)
    {
        if (buildingType == "CityCenter")
        {
            // City centers use dedicated manager
            CityCenterManager.Instance.Build();
        }
        else
        {
            // All other buildings use BuildingManager
            Debug.Log("Trying to build a " + buildingType);
            BuildingManager.Instance.Build(buildingType);
        }
    }

    #endregion
}
