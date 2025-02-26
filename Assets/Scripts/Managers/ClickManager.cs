using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager : MonoBehaviour
{
    public LayerMask tileLayer; // Stellt sicher, dass nur HexTiles getroffen werden!

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Linksklick
        {
            // Falls ein UI-Element angeklickt wurde, ignoriere das Tile-Handling
            if (IsPointerOverUI()) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, tileLayer))
            {
                HexTile clickedTile = hit.collider.GetComponent<HexTile>();
                if (clickedTile != null)
                {
                    ActiveTile.Instance.SetActiveTile(clickedTile);
                }
            }
        }
    }

    // Prüft, ob die Maus über einem UI-Element ist
    private bool IsPointerOverUI()
    {
        //TODO:
        return false;
       // return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}