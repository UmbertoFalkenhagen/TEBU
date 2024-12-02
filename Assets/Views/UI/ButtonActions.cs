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

            ScriptableCityCenter matchingCityCenter = GridMap.Instance.scriptableCityCenters.Find(center => center.cityLocation == hexTile.TileType);
            hexTile.PlaceCityCenterOnTile(matchingCityCenter);
            Debug.Log("City Center built!");

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

        if (hexTile.heldBuilding == null) 
        {
            hexTile.PlaceBuildingOnTile(building);
            Debug.Log($"{building.buildingName} constructed on tile!");
        } else
        {
            Debug.Log("A building already exists on this tile.");
        }
        
        

    }
}
