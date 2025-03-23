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
    public static BuildingPanel Instance { get; private set; }

    //TODO: Bug beheben, dass wenn auf tile mit gebäude geklickt wird und gebaut, auf das tile vorher ohne gebäude gebaut wird
    public Dictionary<string, string> buildingOptions = new Dictionary<string, string> //TODO: Implement to get real information to fill into database
    {
        { "House", "Baue ein Haus" },
        { "Farm", "Errichte eine Farm" },
        { "Barracks", "Baut eine Kaserne" },
        { "CityCenter", "Baue ein Stadtzentrum" }

    };
    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    public void UpdateBuildingOptions()
    {

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
    private void Build(String text)
    {
        //TODO: Dont build if already exist, or disable build menu after build
        BuildingPanel.Instance.onBuildButtonClicked(text);
       
    }

    private void CreateButton(string text, string value)
    {
        //TODO: Add some kind of identifier for Buttons to know which one was klicked
        GameObject newButton = Instantiate(buttonPrefab, buttonContainer);
        newButton.GetComponentInChildren<TextMeshProUGUI>().text = value;
        newButton.GetComponent<Button>().onClick.AddListener(() => Build(text));


    }

    public void onBuildButtonClicked(String buildingType)
    {
        //BuildingButton was clicked > BuildingPanel -> here 
        //check resources in buildingManager?
        UIManager.Instance.RequestBuild(buildingType);
    }
}
