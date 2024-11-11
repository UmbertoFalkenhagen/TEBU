using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TMPro.Examples;  // Namespace für TextMeshPro
using UnityEngine.UI;
public class TileInfoToDisplay : MonoBehaviour
{
    //
    public string tileType;                          // UI-Text-Element für den Typ des Tiles
    private GameObject lastActiveTile;
    private UIElementGenerator uiElementGenerator;
    private ButtonActions buttonActions;
    // The UI Elements that need to be changed
    // Speichert das zuletzt aktive Tile
    // Start is called before the first frame update
    void Start()
    {
        uiElementGenerator = FindObjectOfType<UIElementGenerator>();
        buttonActions = GameObject.FindObjectOfType<ButtonActions>();
    }

    // Update is called once per frame
    void Update()
    {
        GameObject currentActiveTile = ActiveTileController.Instance.getActiveTileGameObject();

        if (currentActiveTile != lastActiveTile)
        {
            lastActiveTile = currentActiveTile;

            if (currentActiveTile != null)
            {
                UpdateTileInfo();
            }
            else
            {
                ClearTileInfo();
            }
        }
    }
    /* Methode zum Aktualisieren der UI-Informationen basierend auf dem Tile
     * Updates UI Based on Clicked Tile
     * 
     * 
    */
    private void UpdateTileInfo()
    {
        // Beispiel: Annahme, dass das Tile über ein Script `HexTile` mit den gewünschten Daten verfügt
        HexTile hexTile = lastActiveTile.GetComponent<HexTile>();
        if (hexTile == null) return;
        List<GameObject> hexTileObjects = GridMap.Instance.FindTilesWithinRange(lastActiveTile, 3);

            if (hexTile.heldBuilding == null)
            {
               if(!IsCityInHeldBuilding(hexTileObjects)) 
                {
                Debug.Log("test");
                    uiElementGenerator.CreateButton(new Vector2(100, 100), "BuildCC", buttonActions.BuildCity);

                }
                else
                {
                    ClearTileInfo();
                }
                //else if(stadt in distance 2 oder 3) { show you cant build here city to close}
                if (true)//keine stadt in distance < 4)
                {

                }

                //show build Options
                // }else if(hexTile.heldBuilding == CityCenter) {}
                //else if(hexTile.heldBuilding == XX) {}

                uiElementGenerator.CreateText(new Vector2(100, 100), "Typ: " + hexTile.resource );

            }
            else
            {
            //TODO: ADD behaviour when city is clicked
                ClearTileInfo();
                uiElementGenerator.CreateText(new Vector2(100, 100), "Hello, im a Building");

             }



    }
    private void ClearTileInfo() {
        uiElementGenerator.DestroyAllUIElements();
    }


    /*
     * Checks if there is a city on any Tile 
     * Params: 
     * List<GameObject> hexTileObjects
     * 
     */
    private bool IsCityInHeldBuilding(List<GameObject> hexTileObjects)
    {
        foreach (GameObject hexTileObject in hexTileObjects)
        {

            HexTile hexTile = hexTileObject.GetComponent<HexTile>();

            // Prüfen, ob das HexTile-Skript existiert und ob heldBuilding "city" ist
            if (hexTile != null && hexTile.heldBuilding != null && hexTile.heldBuilding.name == "CityCenter(Clone)")
            {

                return true;  // Wenn ein HexTile-Objekt "city" enthält, gib true zurück
            }
        }

        return false;  // Falls keines der HexTiles "city" enthält
    }

}
