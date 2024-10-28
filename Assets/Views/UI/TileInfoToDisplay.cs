using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TMPro.Examples;  // Namespace für TextMeshPro
public class TileInfoToDisplay : MonoBehaviour
{
    public ActiveTileController activeTileController;  // Referenz zum ActiveTileController
    //
    public string tileType;                          // UI-Text-Element für den Typ des Tiles


    private GameObject lastActiveTile;

    // The UI Elements that need to be changed
    public TextMeshProUGUI resourceTypeText;
    // Speichert das zuletzt aktive Tile
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Typ: ");

        // Wenn ActiveTileController nicht zugewiesen ist, versuche, ihn automatisch zu finden
        if (activeTileController == null)
        {
            activeTileController = FindObjectOfType<ActiveTileController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
            
        if (activeTileController != null && activeTileController.activeTile != lastActiveTile)
        {
            lastActiveTile = activeTileController.activeTile;
            // Wenn ein aktives Tile vorhanden ist, zeige die Infos an
            if (lastActiveTile != null)
            {
                UpdateTileInfo(lastActiveTile);
            }
            else
            {
                ClearTileInfo();
            }

        }
    }
    // Methode zum Aktualisieren der UI-Informationen basierend auf dem Tile
    private void UpdateTileInfo(GameObject tile)
    {
        // Beispiel: Annahme, dass das Tile über ein Script `HexTile` mit den gewünschten Daten verfügt
        HexTile hexTile = tile.GetComponent<HexTile>();

        if (hexTile != null)
        {


            tileType = "Typ: " + hexTile.resource;
            resourceTypeText = GameObject.Find("ResourceTypeText").GetComponent<TextMeshProUGUI>();
            resourceTypeText.text = tileType;

            Debug.Log(tileType);

        }
    }
    private void ClearTileInfo() {
        Debug.Log("dd");
    }
}
