using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileClick : MonoBehaviour, IClickable, IRightClickable
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
        activeTileController.SetActiveTile(gameObject);
    }      

    public void OnRightClick()
    {
        // Verhalten für rechten Mausklick

        Debug.Log("Einheit wurde rechts angeklickt!");
    }


}
