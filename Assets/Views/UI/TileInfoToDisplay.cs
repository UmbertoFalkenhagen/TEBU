using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TMPro.Examples;  // Namespace für TextMeshPro
public class TileInfoToDisplay : MonoBehaviour
{
    //
    public string tileType;                          // UI-Text-Element für den Typ des Tiles
    private GameObject lastActiveTile;
    private SpawnUIButton spawnUIButton;
    private GameObject inventoryPanel;
    // The UI Elements that need to be changed
    public TextMeshProUGUI resourceTypeText;
    // Speichert das zuletzt aktive Tile
    // Start is called before the first frame update
    void Start()
    {
        spawnUIButton = FindObjectOfType<SpawnUIButton>();
        inventoryPanel = GameObject.Find("UICanvas/InventoryPanel/Grid");
        Debug.Log(inventoryPanel);


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
    // Methode zum Aktualisieren der UI-Informationen basierend auf dem Tile
    private void UpdateTileInfo()
    {
        // Beispiel: Annahme, dass das Tile über ein Script `HexTile` mit den gewünschten Daten verfügt
        HexTile hexTile = lastActiveTile.GetComponent<HexTile>();
        List<GameObject> hexTileObjects = GridMap.Instance.FindTilesWithinRange(lastActiveTile, 3);


        if (hexTile != null)
        {
            if (hexTile.heldBuilding == null)
            {
               if(!IsCityInHeldBuilding(hexTileObjects)) {

                 spawnUIButton.SpawnButtonBuildCity(inventoryPanel.transform); // Beispiel-Position



                }
                //else if(stadt in distance 2 oder 3) { show you cant build here city to close}
                if (true)//keine stadt in distance < 4)
                {
                    Debug.Log("spawn: " + spawnUIButton);

                }

                //show build Options
                // }else if(hexTile.heldBuilding == CityCenter) {}
                //else if(hexTile.heldBuilding == XX) {}

                tileType = "Typ: " + hexTile.resource;
                resourceTypeText = GameObject.Find("ResourceTypeText").GetComponent<TextMeshProUGUI>();
                resourceTypeText.text = tileType;

                Debug.Log(tileType);
            }

        }
    }
    private void ClearTileInfo() {
        Debug.Log("dd");
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
            if (hexTile != null && hexTile.heldBuilding != null && hexTile.heldBuilding.name == "city")
            {
                Debug.Log("CITY FOUND ON: " + hexTileObject.transform.position);
                return true;  // Wenn ein HexTile-Objekt "city" enthält, gib true zurück
            }
        }

        return false;  // Falls keines der HexTiles "city" enthält
    }

}
