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
    HexTile hexTile;
    public string tileType;                          // UI-Text-Element für den Typ des Tiles
    private GameObject activeTile;
    private UIElementGenerator uiElementGenerator;
    private ButtonActions buttonActions;
    private Transform infoPanelGridTransform;
    private Transform buildingPanelGridTransform;
    // The UI Elements that need to be changed
    // Speichert das zuletzt aktive Tile
    // Start is called before the first frame update
    void Start()
    {
        uiElementGenerator = FindObjectOfType<UIElementGenerator>();
        buttonActions = GameObject.FindObjectOfType<ButtonActions>();
        infoPanelGridTransform = GameObject.Find("UICanvas/InfoPanel/Grid").transform;
        buildingPanelGridTransform = GameObject.Find("UICanvas/BuildingPanel/Grid").transform;
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
                hexTile = activeTile.GetComponent<HexTile>();

                UpdateBuildingMenu();
                UpdateInfoMenu();

            }
            else
            {
                ClearTileInfo(infoPanelGridTransform);
                ClearTileInfo(buildingPanelGridTransform);

            }
        }
    }
    private void UpdateInfoMenu()
    {
        ClearTileInfo(infoPanelGridTransform);
        // Display tile type information
        uiElementGenerator.CreateText(new Vector2(100, 100), "Tile Resource: " + hexTile.resource, infoPanelGridTransform);
        if (hexTile.heldBuilding != null)
        {
            Debug.Log(hexTile.heldBuilding.GetComponent<CityCenter>().name);
            if(hexTile.heldBuilding.GetComponent<CityCenter>().name == "CityCenter")
            {
                uiElementGenerator.CreateText(new Vector2(100, 100), "Building: City Center", infoPanelGridTransform);

            }

        }

    }
    /* Methode zum Aktualisieren der UI-Informationen basierend auf dem Tile
     * Updates UI Based on Clicked Tile
     * 
     * 
    */
    private void UpdateBuildingMenu()
    {
        if (hexTile == null) return;

        float xPosition = 100; // Initial x position for placing UI buttons

        // Clear previous UI elements to avoid overlap or overwrite issues
        //ClearTileInfo(GameObject.Find("UICanvas/BuildingPanel/Grid").transform);
        //uiElementGenerator.buildingPanel.SetActive(false);

        // Check if there is a city center adjacent to the selected tile
        bool hasAdjacentCityCenter = IsCityInHeldBuilding(GridMap.Instance.FindTilesWithinRange(activeTile, 1.5f));
        // Check if there is a city center within 3 tiles
        bool cityCenterNearby = IsCityInHeldBuilding(GridMap.Instance.FindTilesWithinRange(activeTile, 3f));

        // Show "Build City Center" button if no city center is within 3 tiles
        if (!cityCenterNearby)
        {

            uiElementGenerator.buildingPanel.SetActive(true);
            ClearTileInfo(buildingPanelGridTransform);


            uiElementGenerator.CreateButton(new Vector2(xPosition, 100), $"Construct CC", buttonActions.BuildCity, buildingPanelGridTransform);
        }
        else if (hexTile.heldBuilding == null && hasAdjacentCityCenter) // if there is no city center close 
        {
            uiElementGenerator.buildingPanel.SetActive(true);
            ClearTileInfo(buildingPanelGridTransform);

            List<ScriptableBuilding> buildingOptions = hexTile.getAllowedBuildings();
            foreach (ScriptableBuilding buildingOption in buildingOptions)
            {
                uiElementGenerator.CreateButton(new Vector2(xPosition, 100), $"Construct {buildingOption.buildingName.ToString()}",() => buttonActions.BuildBuilding(buildingOption), buildingPanelGridTransform);
            }


        }
        else
        {
            uiElementGenerator.buildingPanel.SetActive(false);
        }


    }

    private void ClearTileInfo(Transform parent) {
        uiElementGenerator.DestroyAllUIElements(parent);
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
