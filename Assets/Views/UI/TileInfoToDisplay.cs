using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TMPro.Examples;  // Namespace für TextMeshPro
using UnityEngine.UI;
using System.Runtime.InteropServices.WindowsRuntime;
public class TileInfoToDisplay : MonoBehaviour
{
    //
    public string tileType;                          // UI-Text-Element für den Typ des Tiles
    private GameObject activeTile;
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
        getCurrentTile();
    }

    public void getCurrentTile()
    {
        GameObject newTile = ActiveTileController.Instance.getActiveTileGameObject();

        if (newTile != activeTile)
        {
            activeTile = newTile;

            if (activeTile != null)
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
        HexTile hexTile = activeTile.GetComponent<HexTile>();
        if (hexTile == null) return;

        float xPosition = 100; // Initial x position for placing UI buttons

        // Clear previous UI elements to avoid overlap or overwrite issues
        uiElementGenerator.DestroyAllUIElements();
        uiElementGenerator.buildingPanel.SetActive(false);

        // Check if there is a city center adjacent to the selected tile
        bool hasAdjacentCityCenter = IsCityInHeldBuilding(GridMap.Instance.FindTilesWithinRange(activeTile, 1.5f));
        // Check if there is a city center within 3 tiles
        bool cityCenterNearby = IsCityInHeldBuilding(GridMap.Instance.FindTilesWithinRange(activeTile, 3f));

        // Show "Build City Center" button if no city center is within 3 tiles
        if (!cityCenterNearby)
        {
            uiElementGenerator.buildingPanel.SetActive(true);
            uiElementGenerator.CreateButton(new Vector2(xPosition, 100), $"Construct CC", buttonActions.BuildCity);
        }
        else if (hexTile.heldBuilding == null && hasAdjacentCityCenter) // if there is no city center close 
        {
            List<ScriptableBuilding> buildingOptions = hexTile.getAllowedBuildings();
            foreach (ScriptableBuilding buildingOption in buildingOptions)
            {
                uiElementGenerator.buildingPanel.SetActive(true);
                uiElementGenerator.CreateButton(new Vector2(xPosition, 100), $"Construct {buildingOption.buildingName.ToString()}",() => buttonActions.BuildBuilding(buildingOption));
            }


        }
        else
        {
            uiElementGenerator.buildingPanel.SetActive(false);
        }

        // Display tile type information
        uiElementGenerator.CreateText(new Vector2(100, 100), "Tile Resource: " + hexTile.resource);
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
            if (hexTile != null && hexTile.heldBuilding != null && hexTile.heldBuilding.GetComponent< CityCenter>() != null)
            {

                return true;  // Wenn ein HexTile-Objekt "city" enthält, gib true zurück
            }
        }

        return false;  // Falls keines der HexTiles "city" enthält
    }


}
