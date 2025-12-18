using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #region Complex Building Operations

    public void Build(string inputString)
    {
        HexTile tileToBuildOn = ActiveTile.Instance.GetActiveTile();
        BuildingType _buildingType = System.Enum.TryParse(inputString, true, out BuildingType parsed) ? parsed : default;

        SDBBuildingBlueprintValue buildingBlueprint = DatabaseManager.Instance.GetBuildingBlueprint(_buildingType);
        KeyValuePair<ObjectIdentifier, DBBuildingValue> building = BuildingFactory.Instance.CreateBuilding(_buildingType, buildingBlueprint, tileToBuildOn);

        if (building.Value != null)
        {
            ObjectIdentifier parentCityCenterId = DatabaseManager.Instance.GetTileValue(tileToBuildOn.TileID).ConstructionClaims[0];

            DatabaseManager.Instance.AddBuilding(building.Key, building.Value, parentCityCenterId);
            CreateActiveClaimsForBuilding(building.Key);
        }
        UIManager.Instance.TileClick(ActiveTile.Instance.GetActiveTileID());
    }

    public void CreateActiveClaimsForBuilding(ObjectIdentifier buildingId)
    {
        if (buildingId.Type != ObjectType.Building)
        {
            Debug.LogError($"BuildingManager: Provided ID [{buildingId}] is not a Building.");
            return;
        }

        DatabaseManager db = DatabaseManager.Instance;
        if (!db.BuildingDictionary.TryGetValue(buildingId, out DBBuildingValue buildingValue))
        {
            Debug.LogError($"BuildingManager: Building [{buildingId}] not found in BuildingDictionary.");
            return;
        }

        BuildingType buildingType = buildingValue._type;

        if (!db.buildingBlueprintDictionary.TryGetValue(buildingType, out SDBBuildingBlueprintValue blueprint))
        {
            Debug.LogError($"BuildingManager: No blueprint found for BuildingType [{buildingType}].");
            return;
        }

        ObjectIdentifier parentTileId = db.GetParentTileIdByBuildingId(buildingId);
        if (parentTileId == null)
        {
            Debug.LogError($"BuildingManager: Building [{buildingId}] has no valid parent tile.");
            return;
        }

        DBTileValue parentTileVal = db.GetTileValue(parentTileId);
        if (parentTileVal == null)
        {
            Debug.LogError($"BuildingManager: No DBTileValue found for parent tile [{parentTileId}].");
            return;
        }

        parentTileVal.ActiveClaims.Remove(buildingId);
        parentTileVal.ActiveClaims.Insert(0, buildingId);
        parentTileVal.TileObject.GetComponent<HexTile>().activeClaims.Insert(0, buildingId);

        if (parentTileVal.ActiveClaims.Count > 1)
        {
            PlaceModules(parentTileVal.ActiveClaims[1]);
        }

        buildingValue._claimedTiles.Clear();
        buildingValue._claimedTiles.Add(parentTileId);

        int claimsCreatedCount = 1;

        if (!blueprint.isMaxWorkersFixed)
        {
            List<DBTileValue> adjacentTiles = db.GetTileNeighbors(parentTileId);
            if (adjacentTiles != null)
            {
                foreach (var neighborTile in adjacentTiles)
                {
                    if (!blueprint.requiredTileTypes.Contains(neighborTile.Type))
                        continue;

                    bool resourceRequirementMet =
                        blueprint.requiredResources.Contains(ResourceType.None) ||
                        blueprint.requiredResources.Contains(neighborTile.Resource);

                    if (!resourceRequirementMet)
                        continue;

                    if (!neighborTile.ActiveClaims.Contains(buildingId))
                    {
                        neighborTile.ActiveClaims.Add(buildingId);
                        neighborTile.TileObject.GetComponent<HexTile>().activeClaims.Add(buildingId);
                        buildingValue._claimedTiles.Add(db.GetTileIdByObject(neighborTile.TileObject));
                        claimsCreatedCount++;
                    }
                }
            }
        }

        if (buildingValue._object != null)
        {
            Building buildingComponent = buildingValue._object.GetComponent<Building>();
            if (buildingComponent == null)
            {
                buildingComponent = buildingValue._object.AddComponent<Building>();
            }

            buildingComponent.buildingID = buildingId;
            buildingComponent.buildingType = buildingType;

            if (!blueprint.isMaxWorkersFixed)
            {
                PlaceModules(buildingId);
            }
            else
            {
                buildingValue._currentAnimalWorkerLimit = blueprint.maxWorkers;
            }
        }
        else
        {
            Debug.LogWarning($"BuildingManager: No building GameObject found for {buildingId}.");
        }

        Debug.Log($"BuildingManager: Added building [{buildingId}] as claimant on {claimsCreatedCount} tiles.");
    }

    public void PlaceModules(ObjectIdentifier buildingId)
    {
        if (!DatabaseManager.Instance.BuildingDictionary.TryGetValue(buildingId, out var buildingValue))
        {
            Debug.LogError($"BuildingManager: Building {buildingId} not found");
            return;
        }

        SDBBuildingBlueprintValue blueprint = GetBuildingBlueprint(buildingValue._type);
        if (blueprint == null)
        {
            return;
        }

        if (blueprint.isMaxWorkersFixed)
        {
            return;
        }

        buildingValue._currentAnimalWorkerLimit = 1;

        GameObject workedModulePrefab = blueprint.workedModulePrefab ?? blueprint.unworkedModulePrefab;
        GameObject unworkedModulePrefab = blueprint.unworkedModulePrefab ?? blueprint.prefab;

        if (buildingValue._object == null)
        {
            Debug.LogError($"BuildingManager: Building object is null for {buildingId}");
            return;
        }

        Transform modulesParent = buildingValue._object.transform.Find("Modules");
        if (modulesParent == null)
        {
            GameObject modulesContainer = new GameObject("Modules");
            modulesContainer.transform.SetParent(buildingValue._object.transform);
            modulesContainer.transform.localPosition = Vector3.zero;
            modulesParent = modulesContainer.transform;
        }
        else
        {
            foreach (Transform child in modulesParent)
            {
                Object.Destroy(child.gameObject);
            }
        }

        int workerCount = buildingValue._workers?.Count ?? 0;

        for (int i = 0; i < buildingValue._claimedTiles.Count; i++)
        {
            ObjectIdentifier claimedTileId = buildingValue._claimedTiles[i];
            DBTileValue claimedTile = DatabaseManager.Instance.GetTileValue(claimedTileId);

            if (claimedTile == null || claimedTile.TileObject == null)
            {
                continue;
            }

            bool isWorked = i < workerCount;
            GameObject modulePrefab = isWorked ? workedModulePrefab : unworkedModulePrefab;

            if (modulePrefab != null)
            {
                Vector3 tilePosition = claimedTile.TileObject.transform.position;
                GameObject moduleInstance = Object.Instantiate(modulePrefab, tilePosition, Quaternion.identity, modulesParent);
                moduleInstance.name = $"Module_{i}_{(isWorked ? "Worked" : "Unworked")}";
            }

            if (isWorked)
            {
                buildingValue._currentAnimalWorkerLimit++;
            }
        }

        Debug.Log($"BuildingManager: Placed modules for building {buildingId}. Worker limit: {buildingValue._currentAnimalWorkerLimit}");
    }

    #endregion

    #region Database Wrapper Methods for Subordinate Components

    public DBBuildingValue GetBuildingValue(ObjectIdentifier buildingId)
    {
        return DatabaseManager.Instance.GetBuildingValue(buildingId);
    }

    public ObjectIdentifier GetBuildingIdByTileId(ObjectIdentifier tileId)
    {
        return DatabaseManager.Instance.GetBuildingIdByTileId(tileId);
    }

    public ObjectIdentifier GetParentTileIdByBuildingId(ObjectIdentifier buildingId)
    {
        return DatabaseManager.Instance.GetParentTileIdByBuildingId(buildingId);
    }

    public ObjectIdentifier GetCityCenterIdByBuildingId(ObjectIdentifier buildingId)
    {
        return DatabaseManager.Instance.GetCityCenterIdByBuildingId(buildingId);
    }

    public SDBBuildingBlueprintValue GetBuildingBlueprint(BuildingType buildingType)
    {
        return DatabaseManager.Instance.GetBuildingBlueprint(buildingType);
    }

    public SDBBuildingBlueprintValue GetBuildingBlueprint(string typeName)
    {
        return DatabaseManager.Instance.GetBuildingBlueprint(typeName);
    }

    #endregion
}
