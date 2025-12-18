using TMPro;
using UnityEngine;

public class InfoPanel : MonoBehaviour
{
    #region Singleton

    public static InfoPanel Instance { get; private set; }

    #endregion

    #region Serialized Fields

    [Header("UI References")]
    public TextMeshProUGUI typeLabel;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        gameObject.SetActive(false);
    }

    #endregion

    #region Public Methods

    public void DisplayTileInfo(ObjectIdentifier tileId)
    {
        if (UIManager.Instance == null)
        {
            Debug.LogError("InfoPanel: UIManager instance not found");
            return;
        }

        DBTileValue tileValue = UIManager.Instance.GetTileValue(tileId);

        if (tileValue == null)
        {
            Debug.LogWarning($"InfoPanel: No tile value found for {tileId}");
            Hide();
            return;
        }

        DisplayTileData(tileValue);
    }

    public void DisplayStuff(DBTileValue tileValue)
    {
        DisplayTileData(tileValue);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    #endregion

    #region Private Methods

    private void DisplayTileData(DBTileValue tileValue)
    {
        if (tileValue == null)
        {
            Hide();
            return;
        }

        gameObject.SetActive(true);

        if (typeLabel != null)
        {
            typeLabel.text = $"Type: {tileValue.Type}; Resource: {tileValue.Resource}";
        }
    }

    #endregion
}
