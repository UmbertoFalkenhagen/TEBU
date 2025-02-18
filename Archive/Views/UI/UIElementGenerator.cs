using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
public class UIElementGenerator : MonoBehaviour
{
    // Die Prefabs für die UI-Elemente, die erstellt werden sollen
    public GameObject buttonPrefab;
    public GameObject textPrefab;
    public GameObject buildingPanel;
    private GameObject buildingPanelGrid;
    private GameObject buildingPanelCloseButton;
    public GameObject infoPanel;
    private GameObject infoPanelGrid;

    // Das UI-Element, das als Container für andere Elemente dient (z. B. ein Grid oder Panel)
    private Transform uiParent;
    public void Start()
    {
        buildingPanel = GameObject.Find("UICanvas/BuildingPanel");
        buildingPanelGrid = GameObject.Find("UICanvas/BuildingPanel/Grid");
        buildingPanelCloseButton = GameObject.Find("UICanvas/BuildingPanel/CloseButton");
        infoPanel = GameObject.Find("UICanvas/InfoPanel");
        infoPanelGrid = GameObject.Find("UICanvas/InfoPanel/Grid");

        uiParent = buildingPanelGrid.transform;

    }
    // Methode zum Erstellen eines Buttons
    public GameObject CreateButton(Vector2 position, string buttonText, Action buttonAction, Transform parent)
    {
        GameObject buttonInstance = Instantiate(buttonPrefab, position, Quaternion.identity, parent);
        TextMeshProUGUI buttonTextComponent = buttonInstance.GetComponentInChildren<TextMeshProUGUI>();
        if (buttonTextComponent != null)
        {
            buttonTextComponent.text = buttonText;
        }
        Button button = buttonInstance.GetComponent<Button>();
        button.onClick.AddListener(() => buttonAction.Invoke());
        return buttonInstance;
    } 

    // Methode zum Erstellen von Text
    public GameObject CreateText(Vector2 position, string textContent, Transform parent)
    {
        GameObject textInstance = Instantiate(textPrefab, position, Quaternion.identity, parent);
        var textComponent = textInstance.GetComponent<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = textContent;
        }
        return textInstance;
    }

    // Methode zum Erstellen eines Panels
    public GameObject CreatePanel(Vector2 size)
    {
        GameObject panelInstance = Instantiate(buildingPanelGrid, uiParent);
        RectTransform panelRect = panelInstance.GetComponent<RectTransform>();
        if (panelRect != null)
        {
            panelRect.sizeDelta = size;
        }
        return panelInstance;
    }

    // Methode zum Zerstören eines Buttons
    public void DestroyButton(GameObject button)
    {
        if (button != null)
        {
            Destroy(button);
        }
    }

    // Methode zum Zerstören von Text
    public void DestroyText(GameObject text)
    {
        if (text != null)
        {
            Destroy(text);
        }
    }

    // Methode zum Zerstören eines Panels
    public void DestroyPanel(GameObject panel)
    {
        if (panel != null)
        {
            Destroy(panel);
        }
    }

    // Optional: Zerstört alle UI-Elemente im UI-Parent (z.B. beim Schließen eines Panels oder beim Zurücksetzen)
    public void DestroyAllUIElements(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }
}
