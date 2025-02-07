using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProductRequirement
{
    [Tooltip("The type of product required for production.")]
    public ProductType product;

    [Tooltip("The quantity of the product required for production.")]
    public int quantity;
}
