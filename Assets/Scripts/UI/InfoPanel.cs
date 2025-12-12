using UnityEngine;
using TMPro;
public class InfoPanel : MonoBehaviour
{

    public static InfoPanel Instance { get; private set; }
    public TextMeshProUGUI type;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("InfoPanael here");
        this.gameObject.SetActive(false);
    }

    public void DisplayStuff(DBTileValue tileValue)
    {
        Debug.Log("InfoPanael displaying");
        this.gameObject.SetActive(true);

        type.text = "Type: " + tileValue.Type + "; Resource: " + tileValue.Resource;

        

    }
}
