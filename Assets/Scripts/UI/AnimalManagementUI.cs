using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

/// <summary>
/// Manages the UI Toolkit-based interface for tile information and animal/worker management.
/// Displays a tabbed interface showing:
/// - Tile Info Tab: tile properties, resources, structures, and claims
/// - Management Tab: animal/worker assignment for City Centers and Buildings
/// </summary>
public class AnimalManagementUI : MonoBehaviour
{
    #region Singleton & Serialized Fields

    public static AnimalManagementUI Instance { get; private set; }

    [Header("UI Document References")]
    [SerializeField] private UIDocument uiDocument;

    [Header("Template Assets")]
    [Tooltip("Template for individual animal slots in the management list")]
    [SerializeField] private VisualTreeAsset animalSlotTemplate;

    [Tooltip("Template for selection panel (import/assign animal popup)")]
    [SerializeField] private VisualTreeAsset selectionPanelTemplate;

    #endregion

    #region UI Element References

    // Main container and title
    private VisualElement rootContainer;
    private Label titleLabel;

    // Tab navigation
    private VisualElement tabButtonsContainer;
    private Button tileInfoTabButton;
    private Button managementTabButton;

    // Tab content containers
    private VisualElement tileInfoTab;
    private VisualElement managementTab;

    // Tile info elements
    private VisualElement tileInfoContainer;

    // Management elements
    private VisualElement animalListContainer;
    private Button addAnimalButton;

    // Shared elements
    private Button closeButton;
    private VisualElement selectionPanel;

    #endregion

    #region State Variables

    // Current context tracking
    private ObjectIdentifier currentTileId;
    private ObjectIdentifier currentStructureId;
    private bool hasStructure;

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
    }

    private void OnEnable()
    {
        // Ensure UIDocument reference is set
        if (uiDocument == null)
        {
            uiDocument = GetComponent<UIDocument>();
        }

        // Initialize UI if document is ready
        if (uiDocument != null && uiDocument.rootVisualElement != null)
        {
            SetupUI();
        }
    }

    #endregion

    #region Initialization

    /// <summary>
    /// Queries and caches all UI element references from the UXML document.
    /// Sets up event handlers for buttons and initializes the UI in hidden state.
    /// </summary>
    private void SetupUI()
    {
        var root = uiDocument.rootVisualElement;

        // Query main container and title
        rootContainer = root.Q<VisualElement>("RootContainer");
        titleLabel = root.Q<Label>("TitleLabel");

        // Query tab navigation elements
        tabButtonsContainer = root.Q<VisualElement>("TabButtonsContainer");
        tileInfoTabButton = root.Q<Button>("TileInfoTabButton");
        managementTabButton = root.Q<Button>("ManagementTabButton");

        // Query tab content containers
        tileInfoTab = root.Q<VisualElement>("TileInfoTab");
        managementTab = root.Q<VisualElement>("ManagementTab");

        // Query content elements
        tileInfoContainer = root.Q<VisualElement>("TileInfoContainer");
        animalListContainer = root.Q<VisualElement>("AnimalListContainer");
        addAnimalButton = root.Q<Button>("AddAnimalButton");
        closeButton = root.Q<Button>("CloseButton");

        // Register button click handlers
        if (tileInfoTabButton != null) tileInfoTabButton.clicked += () => SwitchToTab(true);
        if (managementTabButton != null) managementTabButton.clicked += () => SwitchToTab(false);
        if (closeButton != null) closeButton.clicked += HideUI;

        // Start with UI hidden
        HideUI();
    }

    #endregion

    #region Public Entry Points

    /// <summary>
    /// Main entry point called by UIManager when a tile is clicked.
    /// Always displays Tile Info tab first. Management tab is only available if structure exists.
    /// </summary>
    /// <param name="tileId">The tile identifier from DatabaseManager</param>
    public void ShowTileUI(ObjectIdentifier tileId)
    {
        // Store current tile context
        currentTileId = tileId;

        // Check if tile has a structure (City Center or Building)
        currentStructureId = DatabaseManager.Instance.GetStructureId(tileId);
        hasStructure = currentStructureId != null;

        // Show main container
        if (rootContainer != null)
        {
            rootContainer.style.display = DisplayStyle.Flex;
        }

        // Show/hide tab buttons based on structure presence
        // Tabs are only shown when there's a structure to manage
        if (tabButtonsContainer != null)
        {
            tabButtonsContainer.style.display = hasStructure ? DisplayStyle.Flex : DisplayStyle.None;
        }

        // Always start with Tile Info tab when opening UI
        SwitchToTab(true);
        PopulateTileInfo(tileId);
    }

    /// <summary>
    /// Legacy entry point for opening directly to City Center management.
    /// Used when Shift-clicking was the interaction (now replaced by ShowTileUI).
    /// </summary>
    public void ShowCityCenterUI(ObjectIdentifier cityCenterId)
    {
        // Get city data to find parent tile
        DBCityCenterValue cityValue = DatabaseManager.Instance.GetCityCenterValue(cityCenterId);
        if (cityValue == null) return;

        // Set context
        currentTileId = cityValue._parentTile;
        currentStructureId = cityCenterId;
        hasStructure = true;

        // Show UI with tabs
        if (rootContainer != null)
        {
            rootContainer.style.display = DisplayStyle.Flex;
        }

        if (tabButtonsContainer != null)
        {
            tabButtonsContainer.style.display = DisplayStyle.Flex;
        }

        // Go directly to management tab
        SwitchToTab(false);
    }

    /// <summary>
    /// Legacy entry point for opening directly to Building management.
    /// Used when Shift-clicking was the interaction (now replaced by ShowTileUI).
    /// </summary>
    public void ShowBuildingUI(ObjectIdentifier buildingId)
    {
        // Get building data to find parent tile
        DBBuildingValue buildingValue = DatabaseManager.Instance.GetBuildingValue(buildingId);
        if (buildingValue == null) return;

        // Set context
        currentTileId = buildingValue._parentTile;
        currentStructureId = buildingId;
        hasStructure = true;

        // Show UI with tabs
        if (rootContainer != null)
        {
            rootContainer.style.display = DisplayStyle.Flex;
        }

        if (tabButtonsContainer != null)
        {
            tabButtonsContainer.style.display = DisplayStyle.Flex;
        }

        // Go directly to management tab
        SwitchToTab(false);
    }

    /// <summary>
    /// Hides the entire UI and cleans up any open selection panels.
    /// </summary>
    public void HideUI()
    {
        if (rootContainer != null)
        {
            rootContainer.style.display = DisplayStyle.None;
        }

        // Clean up selection panel if it exists
        if (selectionPanel != null)
        {
            selectionPanel.RemoveFromHierarchy();
            selectionPanel = null;
        }
    }

    #endregion

    #region Tab System

    /// <summary>
    /// Switches between Tile Info and Management tabs.
    /// Updates visual state (active tab button styling) and populates appropriate content.
    /// </summary>
    /// <param name="showTileInfo">True for Tile Info tab, False for Management tab</param>
    private void SwitchToTab(bool showTileInfo)
    {
        if (tileInfoTab == null || managementTab == null) return;

        if (showTileInfo)
        {
            // === TILE INFO TAB ===

            // Show tile info, hide management
            tileInfoTab.style.display = DisplayStyle.Flex;
            managementTab.style.display = DisplayStyle.None;

            // Update tab button styling (add active class to current, remove from other)
            if (tileInfoTabButton != null) tileInfoTabButton.AddToClassList("tab-button-active");
            if (managementTabButton != null) managementTabButton.RemoveFromClassList("tab-button-active");

            // Update title
            if (titleLabel != null) titleLabel.text = "Tile Information";

            // Populate tile info content
            if (currentTileId != null)
            {
                PopulateTileInfo(currentTileId);
            }
        }
        else
        {
            // === MANAGEMENT TAB ===

            // Hide tile info, show management
            tileInfoTab.style.display = DisplayStyle.None;
            managementTab.style.display = DisplayStyle.Flex;

            // Update tab button styling
            if (tileInfoTabButton != null) tileInfoTabButton.RemoveFromClassList("tab-button-active");
            if (managementTabButton != null) managementTabButton.AddToClassList("tab-button-active");

            // Populate management content based on structure type
            if (currentStructureId != null)
            {
                if (currentStructureId.Type == ObjectType.CityCenter)
                {
                    PopulateCityCenterManagement(currentStructureId);
                }
                else if (currentStructureId.Type == ObjectType.Building)
                {
                    PopulateBuildingManagement(currentStructureId);
                }
            }
        }
    }

    #endregion

    #region Tile Information Display

    /// <summary>
    /// Populates the Tile Info tab with comprehensive tile data:
    /// - Basic properties (type, resource)
    /// - Structure information (if present)
    /// - Claim lists (active and construction claims)
    /// </summary>
    private void PopulateTileInfo(ObjectIdentifier tileId)
    {
        if (tileInfoContainer == null) return;

        // Clear previous content
        tileInfoContainer.Clear();

        // Fetch tile data from DatabaseManager
        DBTileValue tileValue = DatabaseManager.Instance.GetTileValue(tileId);
        if (tileValue == null)
        {
            tileInfoContainer.Add(CreateInfoLabel("Error", "Tile data not found"));
            return;
        }

        // === BASIC TILE PROPERTIES ===

        tileInfoContainer.Add(CreateInfoLabel("Tile Type", tileValue.Type.ToString()));

        // Resource (display "None" if no resource present)
        string resourceText = tileValue.Resource != ResourceType.None
            ? tileValue.Resource.ToString()
            : "None";
        tileInfoContainer.Add(CreateInfoLabel("Resource", resourceText));

        // === STRUCTURE INFORMATION ===

        if (currentStructureId != null)
        {
            string structureText = "";

            // Format structure text based on type
            if (currentStructureId.Type == ObjectType.CityCenter)
            {
                var cityValue = DatabaseManager.Instance.GetCityCenterValue(currentStructureId);
                structureText = cityValue != null ? $"City Center: {cityValue._cityName}" : "City Center";
            }
            else if (currentStructureId.Type == ObjectType.Building)
            {
                var buildingValue = DatabaseManager.Instance.GetBuildingValue(currentStructureId);
                if (buildingValue != null)
                {
                    Building buildingComponent = buildingValue._object.GetComponent<Building>();
                    structureText = buildingComponent != null
                        ? $"Building: {buildingComponent.buildingType}"
                        : "Building";
                }
            }
            tileInfoContainer.Add(CreateInfoLabel("Structure", structureText));
        }
        else
        {
            tileInfoContainer.Add(CreateInfoLabel("Structure", "None"));
        }

        // === CLAIMS SECTION ===

        tileInfoContainer.Add(CreateSectionHeader("Claims"));

        // Active Claims: Claims from buildings/cities that are currently using this tile
        if (tileValue.ActiveClaims != null && tileValue.ActiveClaims.Count > 0)
        {
            tileInfoContainer.Add(CreateInfoLabel("Active Claims", ""));

            // Display ordered list of active claims
            for (int i = 0; i < tileValue.ActiveClaims.Count; i++)
            {
                ObjectIdentifier claimId = tileValue.ActiveClaims[i];
                string claimText = GetClaimText(claimId);
                tileInfoContainer.Add(CreateIndentedLabel($"{i + 1}. {claimText}"));
            }
        }
        else
        {
            tileInfoContainer.Add(CreateInfoLabel("Active Claims", "None"));
        }

        // Construction Claims: Claims from cities that can build on this tile
        if (tileValue.ConstructionClaims != null && tileValue.ConstructionClaims.Count > 0)
        {
            tileInfoContainer.Add(CreateInfoLabel("Construction Claims", ""));

            // Display ordered list of construction claims
            for (int i = 0; i < tileValue.ConstructionClaims.Count; i++)
            {
                ObjectIdentifier claimId = tileValue.ConstructionClaims[i];
                string claimText = GetClaimText(claimId);
                tileInfoContainer.Add(CreateIndentedLabel($"{i + 1}. {claimText}"));
            }
        }
        else
        {
            tileInfoContainer.Add(CreateInfoLabel("Construction Claims", "None"));
        }
    }

    /// <summary>
    /// Converts a claim ObjectIdentifier into readable text.
    /// Looks up the name/type of the claiming structure.
    /// </summary>
    private string GetClaimText(ObjectIdentifier claimId)
    {
        if (claimId == null) return "Unknown";

        // Format based on claim type
        if (claimId.Type == ObjectType.CityCenter)
        {
            var cityValue = DatabaseManager.Instance.GetCityCenterValue(claimId);
            return cityValue != null ? $"City: {cityValue._cityName}" : "City Center";
        }
        else if (claimId.Type == ObjectType.Building)
        {
            var buildingValue = DatabaseManager.Instance.GetBuildingValue(claimId);
            if (buildingValue != null)
            {
                Building buildingComponent = buildingValue._object.GetComponent<Building>();
                return buildingComponent != null ? $"Building: {buildingComponent.buildingType}" : "Building";
            }
        }

        return claimId.Type.ToString();
    }

    #endregion

    #region Management Display - City Center & Building

    /// <summary>
    /// Populates the Management tab for a City Center.
    /// Displays all animals in the city with their employment status.
    /// Shows "Import Animal" button if city hasn't reached max population (6).
    /// </summary>
    private void PopulateCityCenterManagement(ObjectIdentifier cityCenterId)
    {
        // Fetch city data
        if (!DatabaseManager.Instance.CityCenterDictionary.TryGetValue(cityCenterId, out var cityValue))
        {
            Debug.LogError($"AnimalManagementUI: City center {cityCenterId} not found");
            return;
        }

        // Update title with city name
        if (titleLabel != null)
        {
            titleLabel.text = $"City: {cityValue._cityName}";
        }

        if (animalListContainer != null)
        {
            // Clear previous animal list
            animalListContainer.Clear();

            // Iterate through all animals in this city
            foreach (ObjectIdentifier animalId in cityValue._animals)
            {
                if (!DatabaseManager.Instance.AnimalDictionary.TryGetValue(animalId, out var animalValue))
                {
                    continue;
                }

                // Clone animal slot template
                VisualElement slot = animalSlotTemplate.CloneTree();

                // Query slot elements
                Label nameLabel = slot.Q<Label>("NameLabel");
                Label typeLabel = slot.Q<Label>("TypeLabel");
                Label statusLabel = slot.Q<Label>("StatusLabel");
                Button actionButton = slot.Q<Button>("ActionButton");

                // Populate animal info
                if (nameLabel != null) nameLabel.text = animalValue._animalName;
                if (typeLabel != null) typeLabel.text = $"Type: {animalValue._type}";

                // Display employment status
                if (statusLabel != null)
                {
                    if (animalValue._assignedBuilding != null)
                    {
                        // Animal is employed - show which building
                        DBBuildingValue buildingValue = DatabaseManager.Instance.GetBuildingValue(animalValue._assignedBuilding);
                        statusLabel.text = buildingValue != null
                            ? $"Working at: {buildingValue._type}"
                            : "Employed";
                    }
                    else
                    {
                        // Animal is unemployed
                        statusLabel.text = "Status: Unemployed";
                    }
                }

                // No action button needed in city view (actions happen in building view)
                if (actionButton != null)
                {
                    actionButton.style.display = DisplayStyle.None;
                }

                animalListContainer.Add(slot);
            }
        }

        // Configure "Import Animal" button
        if (addAnimalButton != null)
        {
            addAnimalButton.text = "+ Import Animal from Other City";

            // Only show button if city can accept more animals (max 6 per city)
            addAnimalButton.style.display = AnimalManager.Instance.CanAcceptAnimalInCity(cityCenterId)
                ? DisplayStyle.Flex
                : DisplayStyle.None;

            // Re-register click handler (removes old, adds new to avoid duplicates)
            addAnimalButton.clicked -= OnAddAnimalButtonClicked;
            addAnimalButton.clicked += OnAddAnimalButtonClicked;
        }
    }

    /// <summary>
    /// Populates the Management tab for a Building.
    /// Displays currently assigned workers with "Unassign" buttons.
    /// Shows "Assign Worker" button if building hasn't reached max worker capacity.
    /// </summary>
    private void PopulateBuildingManagement(ObjectIdentifier buildingId)
    {
        // Fetch building data
        if (!DatabaseManager.Instance.BuildingDictionary.TryGetValue(buildingId, out var buildingValue))
        {
            Debug.LogError($"AnimalManagementUI: Building {buildingId} not found");
            return;
        }

        // Get building component to access building type
        Building buildingComponent = buildingValue._object.GetComponent<Building>();

        // Update title with building type
        if (titleLabel != null)
        {
            titleLabel.text = buildingComponent != null
                ? $"Building: {buildingComponent.buildingType}"
                : "Building";
        }

        if (animalListContainer != null)
        {
            // Clear previous worker list
            animalListContainer.Clear();

            // Iterate through assigned workers
            foreach (ObjectIdentifier animalId in buildingValue._workers)
            {
                if (!DatabaseManager.Instance.AnimalDictionary.TryGetValue(animalId, out var animalValue))
                {
                    continue;
                }

                // Clone animal slot template
                VisualElement slot = animalSlotTemplate.CloneTree();

                // Query slot elements
                Label nameLabel = slot.Q<Label>("NameLabel");
                Label typeLabel = slot.Q<Label>("TypeLabel");
                Label statusLabel = slot.Q<Label>("StatusLabel");
                Button actionButton = slot.Q<Button>("ActionButton");

                // Populate worker info
                if (nameLabel != null) nameLabel.text = animalValue._animalName;
                if (typeLabel != null) typeLabel.text = $"Type: {animalValue._type}";

                // Status not needed in building view (all shown animals are workers)
                if (statusLabel != null)
                {
                    statusLabel.style.display = DisplayStyle.None;
                }

                // Configure "Unassign" button to remove worker from building
                if (actionButton != null)
                {
                    actionButton.text = "Unassign";
                    actionButton.AddToClassList("red-button");
                    actionButton.style.display = DisplayStyle.Flex;

                    // Capture animalId and buildingId in closure for click handler
                    actionButton.clicked += () => OnUnassignAnimal(animalId, buildingId);
                }

                animalListContainer.Add(slot);
            }
        }

        // Configure "Assign Worker" button
        if (addAnimalButton != null)
        {
            addAnimalButton.text = "+ Assign Worker";

            // Only show if building can accept more workers (based on blueprint maxWorkers)
            addAnimalButton.style.display = AnimalManager.Instance.CanAcceptWorkerInBuilding(buildingId)
                ? DisplayStyle.Flex
                : DisplayStyle.None;

            // Re-register click handler
            addAnimalButton.clicked -= OnAddAnimalButtonClicked;
            addAnimalButton.clicked += OnAddAnimalButtonClicked;
        }
    }

    #endregion

    #region Selection Panels

    /// <summary>
    /// Handler for the main "Add Animal" / "Assign Worker" button.
    /// Routes to appropriate selection panel based on current structure type.
    /// </summary>
    private void OnAddAnimalButtonClicked()
    {
        if (currentStructureId == null) return;

        if (currentStructureId.Type == ObjectType.CityCenter)
        {
            // Show import panel for city centers
            ShowAnimalImportSelection();
        }
        else if (currentStructureId.Type == ObjectType.Building)
        {
            // Show assignment panel for buildings
            ShowWorkerAssignmentSelection();
        }
    }

    /// <summary>
    /// Displays a selection panel listing unemployed animals from OTHER cities.
    /// User clicks an animal to import it to the current city.
    /// Panel appears to the left of main UI.
    /// </summary>
    private void ShowAnimalImportSelection()
    {
        Debug.Log("ShowAnimalImportSelection called");

        // Remove existing selection panel if present
        if (selectionPanel != null)
        {
            selectionPanel.RemoveFromHierarchy();
        }

        // Clone selection panel template
        selectionPanel = selectionPanelTemplate.CloneTree();

        if (selectionPanel == null)
        {
            Debug.LogError("Failed to clone selection panel template!");
            return;
        }

        // Add panel to root (not to rootContainer, so it appears separately)
        VisualElement root = uiDocument.rootVisualElement;
        root.Add(selectionPanel);

        // Position panel to the left of main panel
        PositionSelectionPanelToLeft();

        // Query selection panel elements
        VisualElement selectionContainer = selectionPanel.Q<VisualElement>("SelectionContainer");
        if (selectionContainer == null)
        {
            Debug.LogError("SelectionContainer not found in selection panel!");
            return;
        }

        Label selectionTitle = selectionContainer.Q<Label>("SelectionTitle");
        VisualElement selectionListContainer = selectionContainer.Q<VisualElement>("SelectionListContainer");
        Button cancelButton = selectionContainer.Q<Button>("CancelButton");

        if (selectionTitle == null || selectionListContainer == null || cancelButton == null)
        {
            Debug.LogError($"Missing UI elements: Title={selectionTitle != null}, List={selectionListContainer != null}, Cancel={cancelButton != null}");
            return;
        }

        // Configure panel
        selectionTitle.text = "Import Animal from Other City";
        selectionListContainer.Clear();

        // Get unemployed animals from other cities (not current city)
        List<ObjectIdentifier> unemployedAnimals = AnimalManager.Instance.GetUnemployedAnimalsInOtherCities(currentStructureId);
        Debug.Log($"Found {unemployedAnimals.Count} unemployed animals in other cities");

        // Populate animal list
        foreach (ObjectIdentifier animalId in unemployedAnimals)
        {
            if (!DatabaseManager.Instance.AnimalDictionary.TryGetValue(animalId, out var animalValue))
            {
                continue;
            }

            // Create selection item with inline styling
            VisualElement selectionItem = new VisualElement();
            selectionItem.AddToClassList("selection-item");
            selectionItem.style.flexDirection = FlexDirection.Column;
            selectionItem.style.paddingTop = 8;
            selectionItem.style.paddingBottom = 8;
            selectionItem.style.paddingLeft = 8;
            selectionItem.style.paddingRight = 8;
            selectionItem.style.marginBottom = 3;
            selectionItem.style.backgroundColor = new Color(0.16f, 0.16f, 0.16f, 0.9f);
            selectionItem.style.borderTopLeftRadius = 3;
            selectionItem.style.borderTopRightRadius = 3;
            selectionItem.style.borderBottomLeftRadius = 3;
            selectionItem.style.borderBottomRightRadius = 3;

            // Animal name
            Label nameLabel = new Label(animalValue._animalName);
            nameLabel.style.fontSize = 14;
            nameLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            nameLabel.style.color = Color.white;

            // Animal type
            Label typeLabel = new Label($"Type: {animalValue._type}");
            typeLabel.style.fontSize = 11;
            typeLabel.style.color = new Color(0.8f, 0.8f, 0.8f);

            // Current city name (where animal is coming from)
            string cityName = "Unknown City";
            if (DatabaseManager.Instance.CityCenterDictionary.TryGetValue(animalValue._parentCityCenter, out var cityValue))
            {
                cityName = cityValue._cityName;
            }

            Label cityLabel = new Label($"City: {cityName}");
            cityLabel.style.fontSize = 11;
            cityLabel.style.color = new Color(0.8f, 0.8f, 0.8f);

            selectionItem.Add(nameLabel);
            selectionItem.Add(typeLabel);
            selectionItem.Add(cityLabel);

            // Click handler to import this animal
            selectionItem.RegisterCallback<ClickEvent>(evt => OnImportAnimal(animalId));

            selectionListContainer.Add(selectionItem);
        }

        // Show message if no animals available
        if (unemployedAnimals.Count == 0)
        {
            Label noAnimalsLabel = new Label("No unemployed animals in other cities");
            noAnimalsLabel.style.color = new Color(0.7f, 0.7f, 0.7f);
            noAnimalsLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            noAnimalsLabel.style.paddingTop = 20;
            noAnimalsLabel.style.paddingBottom = 20;
            selectionListContainer.Add(noAnimalsLabel);
        }

        // Cancel button closes the selection panel
        cancelButton.clicked += () =>
        {
            Debug.Log("Cancel button clicked");
            selectionPanel.RemoveFromHierarchy();
            selectionPanel = null;
        };
    }

    /// <summary>
    /// Displays a selection panel listing unemployed animals in the CURRENT city.
    /// User clicks an animal to assign it as a worker to the current building.
    /// Panel appears to the left of main UI.
    /// </summary>
    private void ShowWorkerAssignmentSelection()
    {
        Debug.Log("ShowWorkerAssignmentSelection called");

        // Remove existing selection panel if present
        if (selectionPanel != null)
        {
            selectionPanel.RemoveFromHierarchy();
        }

        // Clone selection panel template
        selectionPanel = selectionPanelTemplate.CloneTree();

        if (selectionPanel == null)
        {
            Debug.LogError("Failed to clone selection panel template!");
            return;
        }

        // Add panel to root
        VisualElement root = uiDocument.rootVisualElement;
        root.Add(selectionPanel);

        // Position panel to the left of main panel
        PositionSelectionPanelToLeft();

        // Query selection panel elements
        VisualElement selectionContainer = selectionPanel.Q<VisualElement>("SelectionContainer");
        if (selectionContainer == null)
        {
            Debug.LogError("SelectionContainer not found in selection panel!");
            return;
        }

        Label selectionTitle = selectionContainer.Q<Label>("SelectionTitle");
        VisualElement selectionListContainer = selectionContainer.Q<VisualElement>("SelectionListContainer");
        Button cancelButton = selectionContainer.Q<Button>("CancelButton");

        if (selectionTitle == null || selectionListContainer == null || cancelButton == null)
        {
            Debug.LogError($"Missing UI elements: Title={selectionTitle != null}, List={selectionListContainer != null}, Cancel={cancelButton != null}");
            return;
        }

        // Configure panel
        selectionTitle.text = "Assign Worker";
        selectionListContainer.Clear();

        // Validate building exists
        if (!DatabaseManager.Instance.BuildingDictionary.TryGetValue(currentStructureId, out var buildingValue))
        {
            Debug.LogError($"Building {currentStructureId} not found in database");
            return;
        }

        // Find parent city by tracing building -> tile -> construction claims
        DBBuildingValue buildingData = DatabaseManager.Instance.GetBuildingValue(currentStructureId);
        ObjectIdentifier parentTileId = buildingData._parentTile;
        DBTileValue tileValue = DatabaseManager.Instance.GetTileValue(parentTileId);

        if (tileValue == null || tileValue.ConstructionClaims.Count == 0)
        {
            Debug.LogWarning("No construction claims found for building's tile");
            return;
        }

        // First construction claim is the parent city
        ObjectIdentifier parentCityId = tileValue.ConstructionClaims[0];

        // Get unemployed animals in parent city
        List<ObjectIdentifier> unemployedAnimals = AnimalManager.Instance.GetUnemployedAnimalsInCity(parentCityId);
        Debug.Log($"Found {unemployedAnimals.Count} unemployed animals in city {parentCityId}");

        // Populate animal list
        foreach (ObjectIdentifier animalId in unemployedAnimals)
        {
            if (!DatabaseManager.Instance.AnimalDictionary.TryGetValue(animalId, out var animalValue))
            {
                continue;
            }

            // Create selection item with inline styling
            VisualElement selectionItem = new VisualElement();
            selectionItem.AddToClassList("selection-item");
            selectionItem.style.flexDirection = FlexDirection.Column;
            selectionItem.style.paddingTop = 8;
            selectionItem.style.paddingBottom = 8;
            selectionItem.style.paddingLeft = 8;
            selectionItem.style.paddingRight = 8;
            selectionItem.style.marginBottom = 3;
            selectionItem.style.backgroundColor = new Color(0.16f, 0.16f, 0.16f, 0.9f);
            selectionItem.style.borderTopLeftRadius = 3;
            selectionItem.style.borderTopRightRadius = 3;
            selectionItem.style.borderBottomLeftRadius = 3;
            selectionItem.style.borderBottomRightRadius = 3;

            // Animal name
            Label nameLabel = new Label(animalValue._animalName);
            nameLabel.style.fontSize = 14;
            nameLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            nameLabel.style.color = Color.white;

            // Animal type
            Label typeLabel = new Label($"Type: {animalValue._type}");
            typeLabel.style.fontSize = 11;
            typeLabel.style.color = new Color(0.8f, 0.8f, 0.8f);

            selectionItem.Add(nameLabel);
            selectionItem.Add(typeLabel);

            // Click handler to assign this animal
            selectionItem.RegisterCallback<ClickEvent>(evt => OnAssignWorker(animalId));

            selectionListContainer.Add(selectionItem);
        }

        // Show message if no animals available
        if (unemployedAnimals.Count == 0)
        {
            Label noAnimalsLabel = new Label("No unemployed animals available");
            noAnimalsLabel.style.color = new Color(0.7f, 0.7f, 0.7f);
            noAnimalsLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            noAnimalsLabel.style.paddingTop = 20;
            noAnimalsLabel.style.paddingBottom = 20;
            selectionListContainer.Add(noAnimalsLabel);
        }

        // Cancel button closes the selection panel
        cancelButton.clicked += () =>
        {
            Debug.Log("Cancel button clicked");
            selectionPanel.RemoveFromHierarchy();
            selectionPanel = null;
        };
    }

    /// <summary>
    /// Positions the selection panel to the left of the main panel.
    /// Uses absolute positioning with fixed coordinates.
    /// </summary>
    private void PositionSelectionPanelToLeft()
    {
        if (selectionPanel == null || rootContainer == null)
        {
            Debug.LogWarning("Selection panel or root container is null");
            return;
        }

        // Position: 20px from top, 440px from right (leaves 20px gap from main panel)
        selectionPanel.style.position = Position.Absolute;
        selectionPanel.style.top = 20;
        selectionPanel.style.right = 440;
        selectionPanel.style.display = DisplayStyle.Flex;

        Debug.Log($"Selection panel positioned at right: 440px, top: 20px");
    }

    #endregion

    #region Event Handlers

    /// <summary>
    /// Called when user clicks an animal in the import selection panel.
    /// Moves the animal from its current city to the target city.
    /// </summary>
    private void OnImportAnimal(ObjectIdentifier animalId)
    {
        // Validate we're in city center context
        if (currentStructureId == null || currentStructureId.Type != ObjectType.CityCenter)
        {
            Debug.LogError("Cannot import animal: current structure is not a city center");
            return;
        }

        // Execute move operation via AnimalManager
        // This updates DatabaseManager and moves GameObject
        AnimalManager.Instance.MoveAnimalToCity(animalId, currentStructureId);

        // Close selection panel
        if (selectionPanel != null)
        {
            selectionPanel.RemoveFromHierarchy();
            selectionPanel = null;
        }

        // Refresh city management display to show new animal
        PopulateCityCenterManagement(currentStructureId);
    }

    /// <summary>
    /// Called when user clicks an animal in the worker assignment panel.
    /// Assigns the animal as a worker to the current building.
    /// </summary>
    private void OnAssignWorker(ObjectIdentifier animalId)
    {
        // Validate we're in building context
        if (currentStructureId == null || currentStructureId.Type != ObjectType.Building)
        {
            Debug.LogError("Cannot assign worker: current structure is not a building");
            return;
        }

        // Execute assignment via AnimalManager
        // This updates animal's _assignedBuilding, building's _workers list, and moves GameObject
        AnimalManager.Instance.AssignAnimalToBuilding(animalId, currentStructureId);

        // Close selection panel
        if (selectionPanel != null)
        {
            selectionPanel.RemoveFromHierarchy();
            selectionPanel = null;
        }

        // Refresh building management display to show new worker
        PopulateBuildingManagement(currentStructureId);
    }

    /// <summary>
    /// Called when user clicks "Unassign" button on a worker in building view.
    /// Removes the animal from the building and moves it back to city center.
    /// </summary>
    private void OnUnassignAnimal(ObjectIdentifier animalId, ObjectIdentifier buildingId)
    {
        // Execute unassignment via AnimalManager
        // This clears animal's _assignedBuilding, removes from building's _workers, and moves GameObject to city spawn point
        AnimalManager.Instance.UnassignAnimalFromBuilding(animalId);

        // Refresh building management display
        PopulateBuildingManagement(buildingId);
    }

    #endregion

    #region Helper Methods - UI Element Creation

    /// <summary>
    /// Creates a label-value pair display element for tile info.
    /// </summary>
    private VisualElement CreateInfoLabel(string label, string value)
    {
        VisualElement container = new VisualElement();
        container.style.marginBottom = 8;

        Label labelElement = new Label(label + ":");
        labelElement.AddToClassList("info-label");

        Label valueElement = new Label(value);
        valueElement.AddToClassList("info-value");

        container.Add(labelElement);
        container.Add(valueElement);

        return container;
    }

    /// <summary>
    /// Creates a bold section header for tile info categorization.
    /// </summary>
    private Label CreateSectionHeader(string text)
    {
        Label header = new Label(text);
        header.style.fontSize = 16;
        header.style.unityFontStyleAndWeight = FontStyle.Bold;
        header.style.color = new Color(1f, 1f, 1f);
        header.style.marginTop = 15;
        header.style.marginBottom = 8;
        return header;
    }

    /// <summary>
    /// Creates an indented label for list items (used in claims display).
    /// </summary>
    private Label CreateIndentedLabel(string text)
    {
        Label label = new Label(text);
        label.style.fontSize = 12;
        label.style.color = new Color(0.8f, 0.8f, 0.8f);
        label.style.marginLeft = 20;
        label.style.marginBottom = 3;
        return label;
    }

    #endregion
}
