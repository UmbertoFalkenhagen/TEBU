using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public string oName;

    public ObjectIdentifier buildingID;
    public BuildingType buildingType;
    public List<ObjectIdentifier> claimedTiles = new List<ObjectIdentifier>();

    public int animalworkerlimit;
    public bool isMaxWorkersFixed => DatabaseManager.Instance.GetBuildingBlueprint(buildingType).isMaxWorkersFixed;

    public void Awake()
    {
    }

    public void PlaceModules()
    {
        if (isMaxWorkersFixed)
        {
            return;
        }
        animalworkerlimit = 1;
        var blueprint = DatabaseManager.Instance.GetBuildingBlueprint(buildingType);
        GameObject workedModulePrefab;
        if (blueprint.workedModulePrefab != null)
        {
            workedModulePrefab = blueprint.workedModulePrefab;
        } else
        {
            workedModulePrefab = blueprint.unworkedModulePrefab;
        }

            GameObject unworkedModulePrefab = blueprint.unworkedModulePrefab;

        if (workedModulePrefab == null || unworkedModulePrefab == null)
        {
            Debug.LogWarning($"PlaceModules: Missing module prefab(s) for building type {buildingType}");
            return;
        }

        // Count how many animals are currently parented by this building
        int animalCount = 0;
        foreach (var animalEntry in DatabaseManager.Instance.AnimalDictionary)
        {
            if (animalEntry.Value._parentBuilding == buildingID)
            {
                animalCount++;
            }
        }

        int workedPlaced = 0;

        foreach (ObjectIdentifier tileId in claimedTiles)
        {
            DBTileValue tileVal = DatabaseManager.Instance.GetTileValue(tileId);
            if (tileVal == null || tileVal.ActiveClaims.Count == 0 || tileVal.ActiveClaims[0] != buildingID)
                continue;

            // Skip if tile is the parent tile of a city center
            ObjectIdentifier cityCenterId = DatabaseManager.Instance.GetCityCenterIdByTileId(tileId);
            if (cityCenterId != null)
                continue;

            HexTile hexTile = tileVal.TileObject?.GetComponent<HexTile>();
            if (hexTile == null || hexTile.heldBuilding == this.gameObject)
                continue;

            // Choose which prefab to place based on animal capacity
            GameObject modulePrefab = (workedPlaced < animalCount) ? workedModulePrefab : unworkedModulePrefab;

            // Clear tile resource, instantiate module
            hexTile.ClearTileResource();
            hexTile.ClearTileBuilding();
            Vector3 worldPosition = tileVal.TileObject.transform.position;
            GameObject module = Instantiate(modulePrefab, worldPosition, Quaternion.identity, tileVal.TileObject.transform);
            hexTile.heldBuilding = module;

            // Random rotation
            //module.transform.Rotate(0f, Random.Range(0f, 360f), 0f, Space.World);

            // Align and scale module
            Renderer tileRenderer = tileVal.TileObject.GetComponent<Renderer>();
            Renderer moduleRenderer = module.GetComponentInChildren<Renderer>();
            if (tileRenderer != null && moduleRenderer != null)
            {
                Vector3 tileSize = tileRenderer.bounds.size;
                Vector3 moduleSize = moduleRenderer.bounds.size;

                float uniformScale = Mathf.Min(
                    tileSize.x / moduleSize.x,
                    tileSize.z / moduleSize.z
                );
                module.transform.localScale = Vector3.one * uniformScale;

                // Vertical alignment
                float verticalOffset = tileRenderer.bounds.max.y - moduleRenderer.bounds.min.y;
                module.transform.position += new Vector3(0, verticalOffset, 0);
            }

            animalworkerlimit++;

            if (workedPlaced < animalCount)
                workedPlaced++;
        }

        Debug.Log($"PlaceModules: Placed {workedPlaced} worked and {animalworkerlimit - workedPlaced} unworked modules.");
    }
}
