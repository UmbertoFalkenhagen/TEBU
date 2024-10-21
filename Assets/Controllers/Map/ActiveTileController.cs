using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveTileController : MonoBehaviour
{
    // Das aktuell aktive Tile
    public GameObject activeTile;

    // Setzt das neue aktive Tile, wenn ein anderes angeklickt wird
    public void SetActiveTile(GameObject newTile)
    {
        if (activeTile != null)
        {
            // Hier kannst du das alte activeTile deaktivieren oder seine Outline entfernen
            Debug.Log("Deaktiviere altes Tile: " + activeTile.name);
        }

        // Setze das neue aktive Tile
        activeTile = newTile;
        Debug.Log("Neues aktives Tile: " + activeTile.name);

        // Hier kannst du das neue activeTile hervorheben oder seine Outline hinzufügen
    }

}
