using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class BuildingPanel : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform buttonContainer;
    public HexTile activeTile;

    private Dictionary<string, string> buildingOptions = new Dictionary<string, string> //TODO: Implement to get real information to fill into database
    {
        { "House", "Baue ein Haus" },
        { "Farm", "Errichte eine Farm" },
        { "Barracks", "Baut eine Kaserne" },
        { "test", "tttt eine Kaserne" }

    };
    // Start is called before the first frame update
    public void UpdateBuildingOptions(HexTile tile)
    {
        activeTile = tile;
        ClearButtons();

        foreach (var option in buildingOptions)
        {
            CreateButton(option.Key, option.Value);

        }
    }

    private void ClearButtons()
    {
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }
    }
    private void Build()
    {

        //TODO: Dont build if already exist, or disable build menu after build
        ScriptableCityCenter sCC = Resources.Load<ScriptableCityCenter>("Data/Buildings/CityCenter_Grassland");
        activeTile.PlaceCityCenterOnTile(sCC);
    }

    private void CreateButton(string text, string value)
    {
        GameObject newButton = Instantiate(buttonPrefab, buttonContainer);
        newButton.GetComponentInChildren<TextMeshProUGUI>().text = value;
        newButton.GetComponent<Button>().onClick.AddListener(() => Build());


    }
}
