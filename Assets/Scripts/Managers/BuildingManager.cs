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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Build(string inputString)
    {
        Debug.Log("Building " + inputString + "...");
        HexTile tileToBuildOn = ActiveTile.Instance.GetActiveTile();
        BuildingType _buildingType = System.Enum.TryParse(inputString, true, out BuildingType parsed) ? parsed : default;
        Debug.Log("Found building of type " + _buildingType.ToString());
        SDBBuildingBlueprintValue buildingBlueprint = DatabaseManager.Instance.GetBuildingBlueprint(_buildingType);
        KeyValuePair<ObjectIdentifier, DBBuildingValue> building = BuildingFactory.Instance.CreateBuilding(_buildingType, buildingBlueprint, tileToBuildOn);

        if (building.Value != null)
        {
            // e.g. random tile
            DatabaseManager.Instance.AddBuilding(building.Key, building.Value);
            CreateActiveClaimsForBuilding(building.Key);
        }
        UIManager.Instance.tileClick(ActiveTile.Instance.GetActiveTile());
    }

    public void CreateActiveClaimsForBuilding(ObjectIdentifier buildingId)
    {
        // 1) Validate the ObjectIdentifier is for a Building
        if (buildingId.Type != ObjectType.Building)
        {
            Debug.LogError($"CreateActiveClaimsForBuilding: Provided ID [{buildingId}] is not a Building.");
            return;
        }

        // 2) Lookup DBBuildingValue for this building
        DatabaseManager db = DatabaseManager.Instance;
        if (!db.BuildingDictionary.TryGetValue(buildingId, out DBBuildingValue buildingValue))
        {
            Debug.LogError($"CreateActiveClaimsForBuilding: Building [{buildingId}] not found in BuildingDictionary.");
            return;
        }

        // 3) Get the BuildingType from the DBBuildingValue
        BuildingType buildingType = buildingValue._type; // adjust to your field name

        // 4) Retrieve the blueprint from buildingBlueprintDictionary
        if (!db.buildingBlueprintDictionary.TryGetValue(buildingType, out SDBBuildingBlueprintValue blueprint))
        {
            Debug.LogError($"CreateActiveClaimsForBuilding: No blueprint found for BuildingType [{buildingType}].");
            return;
        }

        // 5) Get the tile on which this building sits
        ObjectIdentifier parentTileId = db.GetParentTileIdByBuildingId(buildingId);
        if (parentTileId == null)
        {
            Debug.LogError($"CreateActiveClaimsForBuilding: Building [{buildingId}] has no valid parent tile.");
            return;
        }

        // 6) Gather neighboring tiles around the parent tile
        List<DBTileValue> adjacentTiles = db.GetTileNeighbors(parentTileId);
        if (adjacentTiles == null || adjacentTiles.Count == 0)
        {
            Debug.Log($"CreateActiveClaimsForBuilding: No adjacent tiles found for Building [{buildingId}].");
            return;
        }

        // 7) Attempt to claim each neighbor if it meets tile & resource requirements
        List<DBTileValue> newlyClaimedTiles = new List<DBTileValue>();
        int claimsCreatedCount = 0;

        foreach (DBTileValue neighborTile in adjacentTiles)
        {
            // a) Tile type requirement
            if (!blueprint.requiredTileTypes.Contains(neighborTile.Type))
                continue;

            // b) Resource requirement
            bool resourceRequirementMet = false;
            if (blueprint.requiredResources.Contains(ResourceType.None))
            {
                // "None" in blueprint means no resource restriction
                resourceRequirementMet = true;
            }
            else if (blueprint.requiredResources.Contains(neighborTile.Resource))
            {
                // Otherwise, tile resource must be one of blueprint.requiredResources
                resourceRequirementMet = true;
            }

            if (!resourceRequirementMet)
                continue;

            // c) If not already claimed by this building, add the claim
            if (!neighborTile.ActiveClaims.Contains(buildingId))
            {
                neighborTile.ActiveClaims.Add(buildingId);
                newlyClaimedTiles.Add(neighborTile);
                claimsCreatedCount++;
            }
        }

        //DebugClaimedTilesColoring(buildingId, newlyClaimedTiles);

        Debug.Log($"CreateActiveClaimsForBuilding: Added building [{buildingId}] as claimant on {claimsCreatedCount} tiles.");
    }


    private void DebugClaimedTilesColoring(ObjectIdentifier buildingId, List<DBTileValue> claimedTiles)
    {
        foreach (DBTileValue tileVal in claimedTiles)
        {
            // Only proceed if this building is among the tile's claims
            if (!tileVal.ActiveClaims.Contains(buildingId))
                continue;
            if (tileVal.TileObject == null)
                continue;

            // Grab the renderer
            Renderer tileRenderer = tileVal.TileObject.GetComponent<Renderer>();
            if (tileRenderer == null)
                continue;

            // Check if the tile is actually empty of a building:
            // e.g., if we store this via a 'HexTile' component
            HexTile hexTile = tileVal.TileObject.GetComponent<HexTile>();
            if (hexTile == null)
                continue;

            // Debug color rules
            if (hexTile.heldBuilding == null)
            {
                // If the claiming building is the first in ActiveClaims => RED
                if (tileVal.ActiveClaims.Count > 0 && tileVal.ActiveClaims[0] == buildingId)
                {
                    tileRenderer.material.color = Color.red;
                }
                else
                {
                    // Otherwise => YELLOW
                    tileRenderer.material.color = Color.yellow;
                }
            }
        }
    }
}
