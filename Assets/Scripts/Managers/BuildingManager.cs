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

        // 2) Look up the building’s data
        DatabaseManager db = DatabaseManager.Instance;
        if (!db.BuildingDictionary.TryGetValue(buildingId, out DBBuildingValue buildingValue))
        {
            Debug.LogError($"CreateActiveClaimsForBuilding: Building [{buildingId}] not found in BuildingDictionary.");
            return;
        }

        // 3) Determine the BuildingType
        BuildingType buildingType = buildingValue._type; // adjust if needed

        // 4) Retrieve the corresponding blueprint
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

        DBTileValue parentTileVal = db.GetTileValue(parentTileId);
        if (parentTileVal == null)
        {
            Debug.LogError($"CreateActiveClaimsForBuilding: No DBTileValue found for parent tile [{parentTileId}].");
            return;
        }

        // 6) The building always claims its own tile (and is inserted at index 0)
        //    Remove it first if it was there already, then insert at 0.
        parentTileVal.ActiveClaims.Remove(buildingId);
        parentTileVal.ActiveClaims.Insert(0, buildingId);

        // (We’ll keep track of all claimed tiles for optional debug coloring)
        List<DBTileValue> newlyClaimedTiles = new List<DBTileValue> { parentTileVal };
        int claimsCreatedCount = 1; // We successfully claimed the parent tile

        // 7) If the building’s maxWorkers are not fixed, claim surrounding tiles
        if (!blueprint.isMaxWorkersFixed)
        {
            List<DBTileValue> adjacentTiles = db.GetTileNeighbors(parentTileId);
            if (adjacentTiles != null && adjacentTiles.Count > 0)
            {
                foreach (DBTileValue neighborTile in adjacentTiles)
                {
                    // a) Check tile type requirement
                    if (!blueprint.requiredTileTypes.Contains(neighborTile.Type))
                        continue;

                    // b) Resource requirement
                    bool resourceRequirementMet = false;
                    if (blueprint.requiredResources.Contains(ResourceType.None))
                    {
                        // "None" means no resource restriction
                        resourceRequirementMet = true;
                    }
                    else if (blueprint.requiredResources.Contains(neighborTile.Resource))
                    {
                        resourceRequirementMet = true;
                    }

                    if (!resourceRequirementMet)
                        continue;

                    // c) If not yet claimed by this building, add it
                    if (!neighborTile.ActiveClaims.Contains(buildingId))
                    {
                        neighborTile.ActiveClaims.Add(buildingId);
                        newlyClaimedTiles.Add(neighborTile);
                        claimsCreatedCount++;
                    }
                }
            }
        }
        else
        {
            Debug.Log($"CreateActiveClaimsForBuilding: {buildingType} has isMaxWorkersFixed = true. " +
                      $"Not claiming surrounding tiles, only the parent tile.");
        }

        // 9) (Optional) Debug-color the newly claimed tiles, etc.

        DebugClaimedTilesColoring(buildingId, newlyClaimedTiles);

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
