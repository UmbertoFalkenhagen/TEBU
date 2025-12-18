using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProductDisplay : MonoBehaviour
{
    #region Singleton

    public static ProductDisplay Instance { get; private set; }

    #endregion

    #region Private Fields

    private GameObject productDisplayPrefab;

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
    }

    private void Start()
    {
        LoadProductDisplayPrefab();
        gameObject.SetActive(false);
    }

    #endregion

    #region Initialization

    private void LoadProductDisplayPrefab()
    {
        productDisplayPrefab = Resources.Load<GameObject>("Prefabs/UI/ProductDisplay");

        if (productDisplayPrefab == null)
        {
            Debug.LogError("ProductDisplay: ProductDisplayPrefab not found in Resources/Prefabs/UI/");
        }
    }

    #endregion

    #region Public Methods

    public void UpdateResourceBar(Dictionary<ProductType, int> productInventory)
    {
        Clear();

        if (productInventory == null || productInventory.Count == 0)
        {
            return;
        }

        gameObject.SetActive(true);

        List<(ProductType Type, int Amount)> availableProducts = GetAvailableProducts(productInventory);

        foreach (var product in availableProducts)
        {
            CreateProductDisplay(product.Type, product.Amount);
        }
    }

    public void Clear()
    {
        gameObject.SetActive(false);

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    #endregion

    #region Private Methods

    private List<(ProductType Type, int Amount)> GetAvailableProducts(Dictionary<ProductType, int> productInventory)
    {
        List<(ProductType Type, int Amount)> availableProducts = new List<(ProductType Type, int Amount)>();

        foreach (var kvp in productInventory)
        {
            if (kvp.Value > 0)
            {
                availableProducts.Add((kvp.Key, kvp.Value));
            }
        }

        return availableProducts;
    }

    private void CreateProductDisplay(ProductType productType, int amount)
    {
        if (productDisplayPrefab == null)
        {
            Debug.LogError("ProductDisplay: Cannot create display - prefab is null");
            return;
        }

        GameObject productDisplay = Instantiate(productDisplayPrefab, transform);

        TextMeshProUGUI productText = productDisplay.transform.Find("ProductText")?.GetComponent<TextMeshProUGUI>();

        if (productText != null)
        {
            productText.text = $"{productType}: {amount}";
        }
        else
        {
            Debug.LogWarning("ProductDisplay: ProductText component not found in prefab");
        }
    }

    #endregion
}
