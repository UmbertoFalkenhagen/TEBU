using UnityEngine;
using UnityEngine.UI;

public class SpawnUIButton : MonoBehaviour
{
    public GameObject buttonPrefab; // Das Button-Prefab
    private GameObject buttonInstance; // Die Instanz des Buttons

    // Diese Methode erstellt und platziert den Button zur Laufzeit
    public void SpawnButtonBuildCity(Transform parentTransform)
    {

        // Vergewissere dich, dass das Prefab zugewiesen ist
        if (buttonPrefab == null)
        {
            Debug.LogError("Button Prefab nicht zugewiesen!");
            return;
        }

        // Falls der Button bereits existiert, zerstöre ihn
        if (buttonInstance != null)
        {
            Destroy(buttonInstance);
        }

        // Erstelle den neuen Button und setze ihn als Kind des angegebenen Parent-Objekts
        buttonInstance = Instantiate(buttonPrefab, parentTransform);

        // Setze die Position des Buttons (als RectTransform)
        RectTransform rectTransform = buttonInstance.GetComponent<RectTransform>();

        // Optional: Setze eine Funktion für den Button-Click
        Button button = buttonInstance.GetComponent<Button>();
        button.onClick.AddListener(() => OnButtonClick());
    }

    private void OnButtonClick()
    {
        Debug.Log("Button geklickt!");
        GameObject activeTile = GameObject.Find("InputManager").GetComponent<ActiveTileController>().getActiveTileGameObject();
    }
}