using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public LayerMask clickableLayerTile;

    void Start()
    {
    }
    void Update()
    {
        // Prüfen, ob die linke Maustaste gedrückt wurde
        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }

        // Weitere Mausereignisse wie rechte Maustaste oder Doppelklick können hier abgefragt werden
        if (Input.GetMouseButtonDown(1))
        {
            HandleRightMouseClick();

        }
    }

    void HandleMouseClick()
    {
        // Raycasting, um herauszufinden, was angeklickt wurde
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;


        if (Physics.Raycast(ray, out hit, Mathf.Infinity, clickableLayerTile))
        {
            // Prüfen, ob das angeklickte Objekt ein IClickable-Interface implementiert
            IClickable clickable = hit.collider.GetComponent<IClickable>();
            if (clickable != null)
            {
                clickable.OnClick();
            }
        }
    }

    void HandleRightMouseClick()
    {
        // Hier kannst du das Verhalten für einen Rechtsklick definieren
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Beispiel für die Erkennung von Rechtsklicks auf spezifische Objekte
            IRightClickable rightClickable = hit.collider.GetComponent<IRightClickable>();
            if (rightClickable != null)
            {
                rightClickable.OnRightClick();
            }
        }
    }
}