using System.Collections.Generic;
using UnityEngine;

public class ButtonActions : MonoBehaviour
{
    // Funktion zum Bauen einer Stadt
    public void BuildCity()
    {
        HexTile hexTile = ActiveTileController.Instance.getActiveTileGameObject().GetComponent<HexTile>();
        if (hexTile == null)
        {
            Debug.LogError("HexTile component missing on the selected random tile.");
            return;
        }

        TileType tileType = hexTile.TileType;
        Debug.Log("City wird gebaut!");
        ScriptableCityCenter matchingCityCenter = GridMap.Instance.scriptableCityCenters.Find(center => center.cityLocation == tileType);
        hexTile.PlaceCityCenterOnTile(matchingCityCenter);

        // Weitere Logik zum Bau einer Stadt
    }

    public void BuildBuilding()
    {

    }

    // Funktion für den Angriff auf den Feind
    public void OnAttackEnemy()
    {
        Debug.Log("Angriff auf den Feind!");
        // Weitere Logik zum Angriff auf einen Feind
    }

    // Funktion zum Öffnen des Inventars
    public void OnOpenInventory()
    {
        Debug.Log("Inventar wird geöffnet!");
        // Weitere Logik zum Öffnen des Inventars
    }
}