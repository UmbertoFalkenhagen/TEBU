using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

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

    private VisualElement rootContainer;
    private Label titleLabel;

    private VisualElement tabButtonsContainer;
    private Button tileInfoTabButton;
    private Button managementTabButton;

    private VisualElement tileInfoTab;
    private VisualElement managementTab;

    private VisualElement tileInfoContainer;

    private VisualElement animalListContainer;
    private Button addAnimalButton;

    private Button closeButton;
    private VisualElement selectionPanel;

    #endregion

    #region State Variables

    private ObjectIdentifier currentTileId;
    private ObjectIdentifier currentStructureId;
    private bool hasStructure;

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
    }

    private void OnEnable()
    {
        if (uiDocument == null)
        {
            uiDocument = GetComponent<UIDocument>();
        }

        if (uiDocument != null && uiDocument.rootVisualElement != null)
        {
            SetupUI();
        }
    }

    #endregion

    #region Initialization

    private void SetupUI()
    {
        var root = uiDocument.rootVisualElement;

        rootContainer = root.Q<VisualElement>("RootContainer");
        titleLabel = root.Q<Label>("TitleLabel");

        tabButtonsContainer = root.Q<VisualElement>("TabButtonsContainer");
        tileInfoTabButton = root.Q<Button>("TileInfoTabButton");
        managementTabButton = root.Q<Button>("ManagementTabButton");

        tileInfoTab = root.Q<VisualElement>("TileInfoTab");
        managementTab = root.Q<VisualElement>("ManagementTab");

        tileInfoContainer = root.Q<VisualElement>("TileInfoContainer");
        animalListContainer = root.Q<VisualElement>("AnimalListContainer");
        addAnimalButton = root.Q<Button>("AddAnimalButton");
        closeButton = root.Q<Button>("CloseButton");

        if (tileInfoTabButton != null) tileInfoTabButton.clicked += () => SwitchToTab(true);
        if (managementTabButton != null) managementTabButton.clicked += () => SwitchToTab(false);
        if (closeButton != null) closeButton.clicked += HideUI;

        HideUI();
    }

    #endregion

    #region Public Entry Points

    public void ShowTileUI(ObjectIdentifier tileId)
    {
        currentTileId = tileId;

        currentStructureId = UIManager.Instance.GetStructureId(tileId);
        hasStructure = currentStructureId != null;

        if (rootContainer != null)
        {
            rootContainer.style.display = DisplayStyle.Flex;
        }

        if (tabButtonsContainer != null)
        {
            tabButtonsContainer.style.display = hasStructure ? DisplayStyle.Flex : DisplayStyle.None;
        }

        SwitchToTab(true);
        PopulateTileInfo(tileId);
    }

    public void ShowCityCenterUI(ObjectIdentifier cityCenterId)
    {
        DBCityCenterValue cityValue = UIManager.Instance.GetCityCenterValue(cityCenterId);
        if (cityValue == null) return;

        currentTileId = cityValue._parentTile;
        currentStructureId = cityCenterId;
        hasStructure = true;

        if (rootContainer != null)
        {
            rootContainer.style.display = DisplayStyle.Flex;
        }

        if (tabButtonsContainer != null)
        {
            tabButtonsContainer.style.display = DisplayStyle.Flex;
        }

        SwitchToTab(false);
    }

    public void ShowBuildingUI(ObjectIdentifier buildingId)
    {
        DBBuildingValue buildingValue = UIManager.Instance.GetBuildingValue(buildingId);
        if (buildingValue == null) return;

        currentTileId = buildingValue._parentTile;
        currentStructureId = buildingId;
        hasStructure = true;

        if (rootContainer != null)
        {
            rootContainer.style.display = DisplayStyle.Flex;
        }

        if (tabButtonsContainer != null)
        {
            tabButtonsContainer.style.display = DisplayStyle.Flex;
        }

        SwitchToTab(false);
    }

    public void HideUI()
    {
        if (rootContainer != null)
        {
            rootContainer.style.display = DisplayStyle.None;
        }

        if (selectionPanel != null)
        {
            selectionPanel.RemoveFromHierarchy();
            selectionPanel = null;
        }
    }

    #endregion

    #region Tab System

    private void SwitchToTab(bool showTileInfo)
    {
        if (tileInfoTab == null || managementTab == null) return;

        if (showTileInfo)
        {
            tileInfoTab.style.display = DisplayStyle.Flex;
            managementTab.style.display = DisplayStyle.None;

            if (tileInfoTabButton != null) tileInfoTabButton.AddToClassList("tab-button-active");
            if (managementTabButton != null) managementTabButton.RemoveFromClassList("tab-button-active");

            if (titleLabel != null) titleLabel.text = "Tile Information";

            if (currentTileId != null)
            {
                PopulateTileInfo(currentTileId);
            }
        }
        else
        {
            tileInfoTab.style.display = DisplayStyle.None;
            managementTab.style.display = DisplayStyle.Flex;

            if (tileInfoTabButton != null) tileInfoTabButton.RemoveFromClassList("tab-button-active");
            if (managementTabButton != null) managementTabButton.AddToClassList("tab-button-active");

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

    private void PopulateTileInfo(ObjectIdentifier tileId)
    {
        if (tileInfoContainer == null) return;

        tileInfoContainer.Clear();

        DBTileValue tileValue = UIManager.Instance.GetTileValue(tileId);
        if (tileValue == null)
        {
            tileInfoContainer.Add(CreateInfoLabel("Error", "Tile data not found"));
            return;
        }

        tileInfoContainer.Add(CreateInfoLabel("Tile Type", tileValue.Type.ToString()));

        string resourceText = tileValue.Resource != ResourceType.None
            ? tileValue.Resource.ToString()
            : "None";
        tileInfoContainer.Add(CreateInfoLabel("Resource", resourceText));

        if (currentStructureId != null)
        {
            string structureText = "";

            if (currentStructureId.Type == ObjectType.CityCenter)
            {
                var cityValue = UIManager.Instance.GetCityCenterValue(currentStructureId);
                structureText = cityValue != null ? $"City Center: {cityValue._cityName}" : "City Center";
            }
            else if (currentStructureId.Type == ObjectType.Building)
            {
                var buildingValue = UIManager.Instance.GetBuildingValue(currentStructureId);
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

        tileInfoContainer.Add(CreateSectionHeader("Claims"));

        if (tileValue.ActiveClaims != null && tileValue.ActiveClaims.Count > 0)
        {
            tileInfoContainer.Add(CreateInfoLabel("Active Claims", ""));

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

        if (tileValue.ConstructionClaims != null && tileValue.ConstructionClaims.Count > 0)
        {
            tileInfoContainer.Add(CreateInfoLabel("Construction Claims", ""));

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

    private string GetClaimText(ObjectIdentifier claimId)
    {
        if (claimId == null) return "Unknown";

        if (claimId.Type == ObjectType.CityCenter)
        {
            var cityValue = UIManager.Instance.GetCityCenterValue(claimId);
            return cityValue != null ? $"City: {cityValue._cityName}" : "City Center";
        }
        else if (claimId.Type == ObjectType.Building)
        {
            var buildingValue = UIManager.Instance.GetBuildingValue(claimId);
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

    private void PopulateCityCenterManagement(ObjectIdentifier cityCenterId)
    {
        DBCityCenterValue cityValue = UIManager.Instance.GetCityCenterValue(cityCenterId);
        if (cityValue == null)
        {
            Debug.LogError($"AnimalManagementUI: City center {cityCenterId} not found");
            return;
        }

        if (titleLabel != null)
        {
            titleLabel.text = $"City: {cityValue._cityName}";
        }

        if (animalListContainer != null)
        {
            animalListContainer.Clear();

            foreach (ObjectIdentifier animalId in cityValue._animals)
            {
                DBAnimalValue animalValue = UIManager.Instance.GetAnimalValue(animalId);
                if (animalValue == null) continue;

                VisualElement slot = animalSlotTemplate.CloneTree();

                Label nameLabel = slot.Q<Label>("NameLabel");
                Label typeLabel = slot.Q<Label>("TypeLabel");
                Label statusLabel = slot.Q<Label>("StatusLabel");
                Button actionButton = slot.Q<Button>("ActionButton");

                if (nameLabel != null) nameLabel.text = animalValue._animalName;
                if (typeLabel != null) typeLabel.text = $"Type: {animalValue._type}";

                if (statusLabel != null)
                {
                    if (animalValue._parentBuilding != null)
                    {
                        DBBuildingValue buildingValue = UIManager.Instance.GetBuildingValue(animalValue._parentBuilding);
                        statusLabel.text = buildingValue != null
                            ? $"Working at: {buildingValue._type}"
                            : "Employed";
                    }
                    else
                    {
                        statusLabel.text = "Status: Unemployed";
                    }
                }

                if (actionButton != null)
                {
                    actionButton.style.display = DisplayStyle.None;
                }

                animalListContainer.Add(slot);
            }
        }

        if (addAnimalButton != null)
        {
            addAnimalButton.text = "+ Import Animal from Other City";

            addAnimalButton.style.display = AnimalManager.Instance.CanAcceptAnimalInCity(cityCenterId)
                ? DisplayStyle.Flex
                : DisplayStyle.None;

            addAnimalButton.clicked -= OnAddAnimalButtonClicked;
            addAnimalButton.clicked += OnAddAnimalButtonClicked;
        }
    }

    private void PopulateBuildingManagement(ObjectIdentifier buildingId)
    {
        DBBuildingValue buildingValue = UIManager.Instance.GetBuildingValue(buildingId);
        if (buildingValue == null)
        {
            Debug.LogError($"AnimalManagementUI: Building {buildingId} not found");
            return;
        }

        Building buildingComponent = buildingValue._object.GetComponent<Building>();

        if (titleLabel != null)
        {
            titleLabel.text = buildingComponent != null
                ? $"Building: {buildingComponent.buildingType}"
                : "Building";
        }

        if (animalListContainer != null)
        {
            animalListContainer.Clear();

            foreach (ObjectIdentifier animalId in buildingValue._workers)
            {
                DBAnimalValue animalValue = UIManager.Instance.GetAnimalValue(animalId);
                if (animalValue == null) continue;

                VisualElement slot = animalSlotTemplate.CloneTree();

                Label nameLabel = slot.Q<Label>("NameLabel");
                Label typeLabel = slot.Q<Label>("TypeLabel");
                Label statusLabel = slot.Q<Label>("StatusLabel");
                Button actionButton = slot.Q<Button>("ActionButton");

                if (nameLabel != null) nameLabel.text = animalValue._animalName;
                if (typeLabel != null) typeLabel.text = $"Type: {animalValue._type}";

                if (statusLabel != null)
                {
                    statusLabel.style.display = DisplayStyle.None;
                }

                if (actionButton != null)
                {
                    actionButton.text = "Unassign";
                    actionButton.AddToClassList("red-button");
                    actionButton.style.display = DisplayStyle.Flex;

                    actionButton.clicked += () => OnUnassignAnimal(animalId, buildingId);
                }

                animalListContainer.Add(slot);
            }
        }

        if (addAnimalButton != null)
        {
            addAnimalButton.text = "+ Assign Worker";

            addAnimalButton.style.display = AnimalManager.Instance.CanAcceptWorkerInBuilding(buildingId)
                ? DisplayStyle.Flex
                : DisplayStyle.None;

            addAnimalButton.clicked -= OnAddAnimalButtonClicked;
            addAnimalButton.clicked += OnAddAnimalButtonClicked;
        }
    }

    #endregion

    #region Selection Panels

    private void OnAddAnimalButtonClicked()
    {
        if (currentStructureId == null) return;

        if (currentStructureId.Type == ObjectType.CityCenter)
        {
            ShowAnimalImportSelection();
        }
        else if (currentStructureId.Type == ObjectType.Building)
        {
            ShowWorkerAssignmentSelection();
        }
    }

    private void ShowAnimalImportSelection()
    {
        if (selectionPanel != null)
        {
            selectionPanel.RemoveFromHierarchy();
        }

        selectionPanel = selectionPanelTemplate.CloneTree();

        if (selectionPanel == null)
        {
            Debug.LogError("Failed to clone selection panel template!");
            return;
        }

        VisualElement root = uiDocument.rootVisualElement;
        root.Add(selectionPanel);

        PositionSelectionPanelToLeft();

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
            Debug.LogError($"Missing UI elements in selection panel");
            return;
        }

        selectionTitle.text = "Import Animal from Other City";
        selectionListContainer.Clear();

        List<ObjectIdentifier> unemployedAnimals = AnimalManager.Instance.GetUnemployedAnimalsInOtherCities(currentStructureId);

        foreach (ObjectIdentifier animalId in unemployedAnimals)
        {
            DBAnimalValue animalValue = UIManager.Instance.GetAnimalValue(animalId);
            if (animalValue == null) continue;

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

            Label nameLabel = new Label(animalValue._animalName);
            nameLabel.style.fontSize = 14;
            nameLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            nameLabel.style.color = Color.white;

            Label typeLabel = new Label($"Type: {animalValue._type}");
            typeLabel.style.fontSize = 11;
            typeLabel.style.color = new Color(0.8f, 0.8f, 0.8f);

            string cityName = "Unknown City";
            DBCityCenterValue cityValue = UIManager.Instance.GetCityCenterValue(animalValue._parentCityCenter);
            if (cityValue != null)
            {
                cityName = cityValue._cityName;
            }

            Label cityLabel = new Label($"City: {cityName}");
            cityLabel.style.fontSize = 11;
            cityLabel.style.color = new Color(0.8f, 0.8f, 0.8f);

            selectionItem.Add(nameLabel);
            selectionItem.Add(typeLabel);
            selectionItem.Add(cityLabel);

            selectionItem.RegisterCallback<ClickEvent>(evt => OnImportAnimal(animalId));

            selectionListContainer.Add(selectionItem);
        }

        if (unemployedAnimals.Count == 0)
        {
            Label noAnimalsLabel = new Label("No unemployed animals in other cities");
            noAnimalsLabel.style.color = new Color(0.7f, 0.7f, 0.7f);
            noAnimalsLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            noAnimalsLabel.style.paddingTop = 20;
            noAnimalsLabel.style.paddingBottom = 20;
            selectionListContainer.Add(noAnimalsLabel);
        }

        cancelButton.clicked += () =>
        {
            selectionPanel.RemoveFromHierarchy();
            selectionPanel = null;
        };
    }

    private void ShowWorkerAssignmentSelection()
    {
        if (selectionPanel != null)
        {
            selectionPanel.RemoveFromHierarchy();
        }

        selectionPanel = selectionPanelTemplate.CloneTree();

        if (selectionPanel == null)
        {
            Debug.LogError("Failed to clone selection panel template!");
            return;
        }

        VisualElement root = uiDocument.rootVisualElement;
        root.Add(selectionPanel);

        PositionSelectionPanelToLeft();

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
            Debug.LogError($"Missing UI elements in selection panel");
            return;
        }

        selectionTitle.text = "Assign Worker";
        selectionListContainer.Clear();

        DBBuildingValue buildingValue = UIManager.Instance.GetBuildingValue(currentStructureId);
        if (buildingValue == null)
        {
            Debug.LogError($"Building {currentStructureId} not found");
            return;
        }

        ObjectIdentifier parentTileId = buildingValue._parentTile;
        DBTileValue tileValue = UIManager.Instance.GetTileValue(parentTileId);

        if (tileValue == null || tileValue.ConstructionClaims.Count == 0)
        {
            Debug.LogWarning("No construction claims found for building's tile");
            return;
        }

        ObjectIdentifier parentCityId = tileValue.ConstructionClaims[0];

        List<ObjectIdentifier> unemployedAnimals = AnimalManager.Instance.GetUnemployedAnimalsInCity(parentCityId);

        foreach (ObjectIdentifier animalId in unemployedAnimals)
        {
            DBAnimalValue animalValue = UIManager.Instance.GetAnimalValue(animalId);
            if (animalValue == null) continue;

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

            Label nameLabel = new Label(animalValue._animalName);
            nameLabel.style.fontSize = 14;
            nameLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
            nameLabel.style.color = Color.white;

            Label typeLabel = new Label($"Type: {animalValue._type}");
            typeLabel.style.fontSize = 11;
            typeLabel.style.color = new Color(0.8f, 0.8f, 0.8f);

            selectionItem.Add(nameLabel);
            selectionItem.Add(typeLabel);

            selectionItem.RegisterCallback<ClickEvent>(evt => OnAssignWorker(animalId));

            selectionListContainer.Add(selectionItem);
        }

        if (unemployedAnimals.Count == 0)
        {
            Label noAnimalsLabel = new Label("No unemployed animals available");
            noAnimalsLabel.style.color = new Color(0.7f, 0.7f, 0.7f);
            noAnimalsLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            noAnimalsLabel.style.paddingTop = 20;
            noAnimalsLabel.style.paddingBottom = 20;
            selectionListContainer.Add(noAnimalsLabel);
        }

        cancelButton.clicked += () =>
        {
            selectionPanel.RemoveFromHierarchy();
            selectionPanel = null;
        };
    }

    private void PositionSelectionPanelToLeft()
    {
        if (selectionPanel == null || rootContainer == null)
        {
            Debug.LogWarning("Selection panel or root container is null");
            return;
        }

        selectionPanel.style.position = Position.Absolute;
        selectionPanel.style.top = 20;
        selectionPanel.style.right = 440;
        selectionPanel.style.display = DisplayStyle.Flex;
    }

    #endregion

    #region Event Handlers

    private void OnImportAnimal(ObjectIdentifier animalId)
    {
        if (currentStructureId == null || currentStructureId.Type != ObjectType.CityCenter)
        {
            Debug.LogError("Cannot import animal: current structure is not a city center");
            return;
        }

        AnimalManager.Instance.MoveAnimalToCity(animalId, currentStructureId);

        if (selectionPanel != null)
        {
            selectionPanel.RemoveFromHierarchy();
            selectionPanel = null;
        }

        PopulateCityCenterManagement(currentStructureId);
    }

    private void OnAssignWorker(ObjectIdentifier animalId)
    {
        if (currentStructureId == null || currentStructureId.Type != ObjectType.Building)
        {
            Debug.LogError("Cannot assign worker: current structure is not a building");
            return;
        }

        AnimalManager.Instance.AssignAnimalToBuilding(animalId, currentStructureId);

        if (selectionPanel != null)
        {
            selectionPanel.RemoveFromHierarchy();
            selectionPanel = null;
        }

        PopulateBuildingManagement(currentStructureId);
    }

    private void OnUnassignAnimal(ObjectIdentifier animalId, ObjectIdentifier buildingId)
    {
        AnimalManager.Instance.UnassignAnimalFromBuilding(animalId);

        PopulateBuildingManagement(buildingId);
    }

    #endregion

    #region Helper Methods - UI Element Creation

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
