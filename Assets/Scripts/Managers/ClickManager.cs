using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager : MonoBehaviour
{
    public Camera mainCamera;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Linke Maustaste
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                SelectableObject obj = hit.collider.GetComponent<SelectableObject>();
                if (obj != null)
                {
                    obj.Selected();
                   // UIManager.Instance.UpdateObjectInfo(obj.objectName, obj.value);
                }
            }
        }
    }
}