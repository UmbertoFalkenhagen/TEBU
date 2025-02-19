using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class SelectableObject : MonoBehaviour
{
    public string objectName;
    public string description;
    public string tooltipText;

    private void Awake()
    {
        objectName = "test";
    }
    private void OnMouseDown()
    {
      //  Debug.Log("TestOnMouseDown");
        //if (EventSystem.current.IsPointerOverGameObject())
          //  return; // Verhindert Klicks durch UI

      //  UIManager.Instance.UpdateInfoPanel(objectName, description);
    }
    public void Selected()
    {
     //   if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
     //   {
     //       Debug.Log("Klick ignoriert, weil UI-Element getroffen wurde.");
     //       return;
     //    }

        HandleSelectedID(CheckForPrefix());
    }
    public void HandleSelectedID(string id)
    {
        char prefix = id[0]; // Nimmt das erste Zeichen als Prefix

        switch (prefix)
        {
            case '$':
                Debug.Log($"HexTile gefunden! TypeID: {id}");

                // HandleDollarPrefix(id);
                break;
            case '#':
             //   HandleHashPrefix(id);
                break;
            case '+':
              //  HandlePlusPrefix(id);
                break;
            case '~':
              //  HandleTildePrefix(id);
                break;
            default:
                Debug.LogWarning($"Unbekanntes Prefix: {prefix}");
                break;
        }
    }

    private void OnMouseEnter()
    {
      //  TooltipManager.Instance.ShowTooltip(tooltipText, Input.mousePosition);
    }

    private void OnMouseExit()
    {
       // TooltipManager.Instance.HideTooltip();
    }

    private string CheckForPrefix()
    {
        HexTile hexTile = GetComponent<HexTile>();
        if (hexTile != null)
        {
            return hexTile.TileID.ToString();
        }
        else
        {
            Debug.Log("Kein HexTile-Skript auf diesem Objekt.");
            return null;
        }
    }
}
