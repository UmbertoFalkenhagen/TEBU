using System.Collections.Generic;
using UnityEngine;

public class CityCenter : MonoBehaviour
{

    // Inventory of products available in the city
    public Dictionary<ProductType, int> productInventory = new Dictionary<ProductType, int>();
    public string oName;
    private List<Building> cityBuildings;

    // Initialize the city center with given data
    public void Initialize(ScriptableCityCenter data)
    {
        cityBuildings = new List<Building>();
        // Initialize product inventory (e.g., start with no products)
        foreach (ProductType product in System.Enum.GetValues(typeof(ProductType)))
        {
            productInventory[product] = 0;
        }
        oName = "City Center";
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

    public void AddBuildingToCity(Building _building)
    {
        cityBuildings.Add(_building);
    }

    public void RemoveBuildingFromCity(Building _building)
    {
        cityBuildings.Remove(_building);
    }
}
