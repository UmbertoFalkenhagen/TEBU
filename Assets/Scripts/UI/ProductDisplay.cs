using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ProductDisplay : MonoBehaviour
{
    public static ProductDisplay Instance { get; private set; }
    private GameObject productDisplayPrefab;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }

        Instance = this;
        DontDestroyOnLoad(gameObject);



    }
    private void Start()
    {
        //
        //add ProductDisplay prefab
        productDisplayPrefab = Resources.Load<GameObject>("Prefabs/UI/ProductDisplay"); // Path inside Resources folder
        if (productDisplayPrefab != null)
        {
            Instantiate(productDisplayPrefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.LogError("productDisplayPrefab not found!");
        }
        this.gameObject.SetActive(false);


    }

    public void UpdateResourceBar(Dictionary<ProductType, int> productInventory)
    {
        Clear();
        this.gameObject.SetActive(true);
        List<(ProductType Type, int Amount)> availableProducts = GetAvailableProducts(productInventory);
        foreach (var product in availableProducts)
        {
            GameObject productDisplay = Instantiate(productDisplayPrefab, this.transform);
            TextMeshProUGUI productText = productDisplay.transform.Find("ProductText").GetComponent<TextMeshProUGUI>(); // Nutze Find(), um die Text-Komponente zu finden

            if (productText != null)
            {
                // Setze den Text mit dem Produkttyp und der Menge
                productText.text = $"{product.Type}: {product.Amount}";
            }
        }
    }


    public List<(ProductType Type, int Amount)> GetAvailableProducts(Dictionary<ProductType, int> productInventory)
    {
        List<(ProductType Type, int Amount)> availableProducts = new List<(ProductType Type, int Amount)>();

        foreach (var kvp in productInventory)
        {
            if (kvp.Value > 0) // Prüfen, ob der Wert größer als 0 ist
            {
                availableProducts.Add((kvp.Key, kvp.Value)); // Tuple (Type, Amount) hinzufügen
            }
        }

        return availableProducts; // Liste der verfügbaren Produkte zurückgeben
    }

    public void Clear()
    {
        this.gameObject.SetActive(false);
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
    }

}
