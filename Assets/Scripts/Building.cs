using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public string oName;

    public ObjectIdentifier buildingID;

    public BuildingType buildingType;

    public List<ObjectIdentifier> claimedTiles = new List<ObjectIdentifier>();

    public void Awake()
    {
        
    }

    public void PlaceUnworkedModules()
    {
        GameObject unworkedModulePrefab = DatabaseManager.Instance.GetBuildingBlueprint(buildingType).unworkedModulePrefab;
        if (unworkedModulePrefab == null)
        {
            return;
        }
        for (int i = 1; i < claimedTiles.Count; i++)
        {
            ObjectIdentifier tile = claimedTiles[i];
            DBTileValue tileVal = DatabaseManager.Instance.GetTileValue(tile);

            if (tileVal.ActiveClaims.Count > 0 && tileVal.ActiveClaims[0] == buildingID && DatabaseManager.Instance.GetStructureIdByTileId(claimedTiles[i]) == null)
            {
                Vector3 worldPosition = tileVal.TileObject.transform.position;
                // Instantiate the module prefab on the tile
                tileVal.TileObject.GetComponent<HexTile>().ClearTileResource();
                // Instantiate the module prefab as a child of the tile
                GameObject buildingObject = Instantiate(unworkedModulePrefab, worldPosition, Quaternion.identity, tileVal.TileObject.transform);

                // Apply a random Y rotation
                float randomYRotation = Random.Range(0f, 360f);
                buildingObject.transform.Rotate(0f, randomYRotation, 0f, Space.World);

                // Align and scale
                Renderer tileRenderer = tileVal.TileObject.GetComponent<Renderer>();
                Renderer moduleRenderer = buildingObject.GetComponentInChildren<Renderer>();

                if (tileRenderer != null && moduleRenderer != null)
                {
                    // --- Step 1: Optional - Scale to match tile footprint ---
                    Vector3 tileSize = tileRenderer.bounds.size;
                    Vector3 moduleSize = moduleRenderer.bounds.size;

                    float uniformScale = Mathf.Min(
                        tileSize.x / moduleSize.x,
                        tileSize.z / moduleSize.z
                    );

                    buildingObject.transform.localScale = Vector3.one * uniformScale;

                    // Recalculate module bounds after scaling
                    moduleRenderer = buildingObject.GetComponentInChildren<Renderer>();
                    moduleSize = moduleRenderer.bounds.size;

                    // --- Step 2: Vertical alignment ---
                    float tileTopY = tileRenderer.bounds.max.y;
                    float moduleBottomY = moduleRenderer.bounds.min.y;
                    float verticalOffset = tileTopY - moduleBottomY;

                    buildingObject.transform.position += new Vector3(0, verticalOffset, 0);
                }


                Debug.Log("Placing an unworked module prefab for " + buildingType.ToString());
            }
        }
    }
}
