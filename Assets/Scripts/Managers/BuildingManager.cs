using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    private SDBBuildingBlueprintValue buildingBlueprint;
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
       // Debug.Log("Building " + inputString + "...");
        HexTile tileToBuildOn = ActiveTile.Instance.GetActiveTile();
        BuildingType _buildingType = System.Enum.TryParse(inputString, true, out BuildingType parsed) ? parsed : default;
       // Debug.Log("Found building of type " + _buildingType.ToString());
        buildingBlueprint = DatabaseManager.Instance.GetBuildingBlueprint(_buildingType);
        KeyValuePair<ObjectIdentifier, DBBuildingValue> building = BuildingFactory.Instance.CreateBuilding(_buildingType, buildingBlueprint, tileToBuildOn);

        if (building.Value != null)
        {
            // e.g. random tile
            ObjectIdentifier parentCityCenterId = DatabaseManager.Instance.GetTileValue(tileToBuildOn.TileID).ConstructionClaims[0];

            DatabaseManager.Instance.AddBuilding(building.Key, building.Value, parentCityCenterId);
            CreateActiveClaimsForBuilding(building.Key);
        }
        UIManager.Instance.TileClick(ActiveTile.Instance.GetActiveTileID());
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
        parentTileVal.TileObject.GetComponent<HexTile>().activeClaims.Insert(0,buildingId);
        if (parentTileVal.ActiveClaims.Count > 1)
        {
            DatabaseManager.Instance.GetBuildingValue(parentTileVal.ActiveClaims[1])._object.GetComponent<Building>().PlaceModules();
        }
        


        List<DBTileValue> newlyClaimedTiles = new List<DBTileValue> { parentTileVal };
        List<ObjectIdentifier> claimedTileIds = new List<ObjectIdentifier> { parentTileId };
        int claimsCreatedCount = 1;

        // 7) If max workers are not fixed, claim surrounding tiles
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
                        newlyClaimedTiles.Add(neighborTile);
                        claimedTileIds.Add(db.GetTileIdByObject(neighborTile.TileObject));
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

        // 8) Store claimed tile IDs in Building component
        if (buildingValue._object != null)
        {
            Building buildingComponent = buildingValue._object.GetComponent<Building>();
            if (buildingComponent == null)
            {
                buildingComponent = buildingValue._object.AddComponent<Building>();
            }

            buildingComponent.buildingID = buildingId;
            buildingComponent.buildingType = buildingType;
            buildingComponent.claimedTiles = claimedTileIds;

            // Only place modules if building can assign workers dynamically
            if (!blueprint.isMaxWorkersFixed)
            {
                buildingComponent.PlaceModules();
            } else
            {
                buildingComponent.animalworkerlimit = buildingBlueprint.maxWorkers;
            }
        }
        else
        {
            Debug.LogWarning($"CreateActiveClaimsForBuilding: No building GameObject found for {buildingId}.");
        }

        // 9) (Optional) Debug-color the newly claimed tiles, etc.

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
