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
        // Pr�fen, ob die linke Maustaste gedr�ckt wurde
        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }

        // Weitere Mausereignisse wie rechte Maustaste oder Doppelklick k�nnen hier abgefragt werden
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
            // Pr�fen, ob das angeklickte Objekt ein IClickable-Interface implementiert
            IClickable clickable = hit.collider.GetComponent<IClickable>();
            if (clickable != null)
            {
                clickable.OnClick();
            }
        }
    }

    void HandleRightMouseClick()
    {
        // Hier kannst du das Verhalten f�r einen Rechtsklick definieren
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Beispiel f�r die Erkennung von Rechtsklicks auf spezifische Objekte
            IRightClickable rightClickable = hit.collider.GetComponent<IRightClickable>();
            if (rightClickable != null)
            {
                rightClickable.OnRightClick();
            }
        }
    }
}