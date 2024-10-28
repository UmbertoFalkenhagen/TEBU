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
                // Hier kannst du das alte activeTile deaktivieren 

                //Debug.Log("Deaktiviere altes Tile: " + activeTile.name);
                activeTile.transform.position = new Vector3(activeTile.transform.position.x, activeTile.transform.position.y - 1, activeTile.transform.position.z);
            }

            // Setze das neue aktive Tile
            activeTile = newTile;
            activeTile.transform.position = new Vector3(activeTile.transform.position.x, activeTile.transform.position.y + 1, activeTile.transform.position.z);

           //Debug.Log("Neues aktives Tile: " + activeTile.name);

            // Hier kannst du das neue activeTile hervorheben
        

    }


}
