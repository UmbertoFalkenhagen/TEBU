using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;

public class UIClick : MonoBehaviour, IClickable, IRightClickable
{
    private ActiveTileController activeTileController;
    void Start()
    {
        // Wenn der ActiveTileController nicht manuell im Inspector zugewiesen wurde, versuche ihn in der Szene zu finden
        if (activeTileController == null)
        {
            activeTileController = FindObjectOfType<ActiveTileController>();
        }
    }
    public void OnClick()
    {
        // Verhalten für linken Mausklick
        Debug.Log("ONCLICddK UI");
    }      

    public void OnRightClick()
    {
        // Verhalten für rechten Mausklick

        Debug.Log("Einheit wurde rechts angeklickt!");
    }


}
