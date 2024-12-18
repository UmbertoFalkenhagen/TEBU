using System.Collections.Generic;
using UnityEngine;

public class CityCenter : MonoBehaviour
{
    public ScriptableCityCenter cityCenterData;

    // Inventory of products available in the city
    public Dictionary<ProductType, int> productInventory = new Dictionary<ProductType, int>();

    // Initialize the city center with given data
    public void Initialize(ScriptableCityCenter data)
    {
        cityCenterData = data;

        // Initialize product inventory (e.g., start with no products)
        foreach (ProductType product in System.Enum.GetValues(typeof(ProductType)))
        {
            productInventory[product] = 0;
        }
    }

    // Method to add products to the inventory
    public void AddProduct(ProductType product, int quantity)
    {
        if (productInventory.ContainsKey(product))
        {
            productInventory[product] += quantity;
        }
        else
        {
            productInventory[product] = quantity;
        }
    }
}
