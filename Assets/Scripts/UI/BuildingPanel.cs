using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildingPanel : MonoBehaviour
{
    #region Singleton

    public static BuildingPanel Instance { get; private set; }

    #endregion

    #region Serialized Fields

    [Header("UI References")]
    public GameObject buttonPrefab;
    public Transform buttonContainer;

    #endregion

    #region State

    public Dictionary<string, string> buildingOptions = new Dictionary<string, string>();

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #endregion

    #region Public Methods

    public void UpdateBuildingOptions()
    {
        ClearButtons();

        if (buildingOptions.Count <= 0)
        {
            return;
        }

        gameObject.SetActive(true);

        foreach (var option in buildingOptions)
        {
            CreateButton(option.Key, option.Value);
        }
    }

    #endregion

    #region Private Methods

    private void ClearButtons()
    {
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }
        gameObject.SetActive(false);
    }

    private void CreateButton(string buildingTypeKey, string displayText)
    {
        GameObject newButton = Instantiate(buttonPrefab, buttonContainer);
        TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();

        if (buttonText != null)
        {
            buttonText.text = displayText;
        }

        Button button = newButton.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => OnBuildButtonClicked(buildingTypeKey));
        }
    }

    private void OnBuildButtonClicked(string buildingType)
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.RequestBuild(buildingType);
        }
        else
        {
            Debug.LogError("BuildingPanel: UIManager instance not found");
        }
    }

    #endregion
}
