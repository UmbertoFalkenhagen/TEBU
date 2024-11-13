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
        HexTile hexTile = lastActiveTile.GetComponent<HexTile>();
        if (hexTile == null) return;

        float xPosition = 100; // Initial x position for placing UI buttons

        // Clear previous UI elements to avoid overlap or overwrite issues
        uiElementGenerator.DestroyAllUIElements();

        // Check if there is a city center adjacent to the selected tile
        bool hasAdjacentCityCenter = IsCityInHeldBuilding(GridMap.Instance.FindTilesWithinRange(lastActiveTile, 1.5f));

        // Check if there is a city center within 3 tiles
        bool cityCenterNearby = IsCityInHeldBuilding(GridMap.Instance.FindTilesWithinRange(lastActiveTile, 3f));

        // Show "Build City Center" button if no city center is within 3 tiles
        if (hexTile.heldBuilding == null && !cityCenterNearby)
        {
            uiElementGenerator.CreateButton(new Vector2(xPosition, 100), "BuildCC", buttonActions.BuildCity);
        }

        // If a city center is adjacent, check for all constructible buildings
        else if (hasAdjacentCityCenter)
        {
            List<string> buildingOptions = new List<string>(); // To store building names for logging

            foreach (var building in GridMap.Instance.scriptableBuildings)
            {
                // Check if the building can be placed on this tile based on its type and required resources
                if (building.suitableTileTypeLocations.Contains(hexTile.TileType) && HasRequiredResources(hexTile, building.requiredResources))
                {
                    // Log the building option and add its name to the list for debug logging
                    buildingOptions.Add(building.buildingName.ToString());

                    // Create a button for each eligible building with a unique action
                    var buildingButton = uiElementGenerator.CreateButton(
                        new Vector2(xPosition, 100),
                        $"Construct {building.buildingName}",
                        () => buttonActions.BuildBuilding(building) // Assign this building to the button’s action
                    );

                    if (buildingButton != null)
                    {
                        xPosition += 150; // Adjust x position for each new button to align them horizontally
                    }
                }
            }

            // Debug log for available building options on this tile
            if (buildingOptions.Count > 0)
            {
                Debug.Log($"Available building options on selected tile: {string.Join(", ", buildingOptions)}");
            }
            else
            {
                Debug.Log("No available building options on the selected tile.");
            }
        }

        // Display tile type information
        uiElementGenerator.CreateText(new Vector2(100, 100), "Tile Type: " + hexTile.resource);
    }

    // Helper method to check for required resources on selected or adjacent tiles
    private bool HasRequiredResources(HexTile hexTile, List<ResourceType> requiredResources)
    {
        if (requiredResources.Contains(hexTile.resource)) return true;

        foreach (var adjacentTileObj in hexTile.adjacentTiles)
        {
            HexTile adjacentTile = adjacentTileObj.GetComponent<HexTile>();
            if (adjacentTile != null && adjacentTile.heldBuilding == null && requiredResources.Contains(adjacentTile.resource))
            {
                return true;
            }
        }
        return false;
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
