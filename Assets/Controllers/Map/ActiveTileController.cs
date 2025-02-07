using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveTileController : MonoBehaviour
{
    #region Singleton
    public static ActiveTileController Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Destroy any duplicate GridMap instances
            return; // Prevents further initialization in this instance
        }
    }
    #endregion
    // Das aktuell aktive Tile
    public GameObject activeTile;

    // Setzt das neue aktive Tile, wenn ein anderes angeklickt wird
    public void SetActiveTile(GameObject newTile)
    {

            if (activeTile != null)
            {
            // Hier kannst du das alte activeTile deaktivieren 
            activate();
                //Debug.Log("Deaktiviere altes Tile: " + activeTile.name);
            }

            // Setze das neue aktive Tile
            activeTile = newTile;

        deactivate();
           //Debug.Log("Neues aktives Tile: " + activeTile.name);

        // Hier kannst du das neue activeTile hervorheben


    }
    private void activate()
    {
        //Debug.Log("Deaktiviere altes Tile: " + activeTile.name);
        activeTile.transform.position = new Vector3(activeTile.transform.position.x, activeTile.transform.position.y - 1, activeTile.transform.position.z);
    }
    private void deactivate()
    {
        activeTile.transform.position = new Vector3(activeTile.transform.position.x, activeTile.transform.position.y + 1, activeTile.transform.position.z);

    }
    public void forceDeactivate()
    {
        deactivate();
        activeTile = null;
    }
    public GameObject getActiveTileGameObject()
    {
        return activeTile;
    }


}
