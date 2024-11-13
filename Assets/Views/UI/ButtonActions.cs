using System.Collections.Generic;
using UnityEngine;

public class ButtonActions : MonoBehaviour
{
    public void BuildCity()
    {
        HexTile hexTile = ActiveTileController.Instance.getActiveTileGameObject().GetComponent<HexTile>();
        if (hexTile == null)
        {
            Debug.LogError("HexTile component missing on the selected tile.");
            return;
        }

        if (!IsCityWithinRange(hexTile, 3))
        {
            ScriptableCityCenter matchingCityCenter = GridMap.Instance.scriptableCityCenters.Find(center => center.cityLocation == hexTile.TileType);
            hexTile.PlaceCityCenterOnTile(matchingCityCenter);
            Debug.Log("City Center built!");
        }
        else
        {
            Debug.LogWarning("Cannot build city center: another city center is too close.");
        }
    }

    // Function to build the specified building on the selected tile
    public void BuildBuilding(ScriptableBuilding building)
    {
        HexTile hexTile = ActiveTileController.Instance.getActiveTileGameObject().GetComponent<HexTile>();
        if (hexTile == null)
        {
            Debug.LogError("HexTile component missing on the selected tile.");
            return;
        }

        // Verify that the tile type and resources allow construction of this specific building
        if (building.suitableTileTypeLocations.Contains(hexTile.TileType) && HasRequiredResources(hexTile, building.requiredResources))
        {
            hexTile.PlaceBuildingOnTile(building);
            Debug.Log($"{building.buildingName} constructed on tile!");
        }
        else
        {
            Debug.LogWarning($"Cannot construct {building.buildingName}: requirements not met.");
        }
    }

    // Helper to check for city centers within a specified range
    private bool IsCityWithinRange(HexTile hexTile, float range)
    {
        List<GameObject> tilesWithinRange = GridMap.Instance.FindTilesWithinRange(hexTile.gameObject, range);
        return tilesWithinRange.Exists(tile => tile.GetComponent<HexTile>().heldBuilding?.name == "CityCenter(Clone)");
    }

    // Helper method to check if required resources are present on the selected or adjacent tiles
    private bool HasRequiredResources(HexTile hexTile, List<ResourceType> requiredResources)
    {
        if (requiredResources.Contains(hexTile.resource)) return true;

        foreach (var adjacentTileObj in hexTile.adjacentTiles)
        {
            HexTile adjacentTile = adjacentTileObj.GetComponent<HexTile>();
            if (adjacentTile != null && adjacentTile.heldBuilding == null && requiredResources.Contains(adjacentTile.resource))
            {
                return true;
            }
        }
        return false;
    }
}
