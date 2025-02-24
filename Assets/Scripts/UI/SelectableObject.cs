using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static Unity.Burst.Intrinsics.X86.Avx;
public class SelectableObject : MonoBehaviour
{
    public string objectName;
    public string description;
    public string tooltipText;
    private UIManager uiManager;
    private void Awake()
    {
        objectName = "test";
        uiManager = UIManager.Instance;
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

        CheckComponentAndExecute();

        //   if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
        //   {
        //       Debug.Log("Klick ignoriert, weil UI-Element getroffen wurde.");
        //       return;
        //    }
        //UIManagerChangeUI based on object
    }
    private void OnMouseEnter()
    {
      //  TooltipManager.Instance.ShowTooltip(tooltipText, Input.mousePosition);
    }

    private void OnMouseExit()
    {
       // TooltipManager.Instance.HideTooltip();
    }
    void CheckComponentAndExecute()
    {
        if (this.TryGetComponent(out HexTile tile))
        {
            ObjectIdentifier tileID = tile.TileID;
            uiManager.tileClick(tileID);
        }
        else if (this.TryGetComponent(out HexMapManager building)) //placeholder for building
        {
           // HandleComponentB(compB);
        }
        else
        {
            Debug.Log("Keines der gesuchten Skripte gefunden.");
        }
    }


}
