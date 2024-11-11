using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewScriptableProduct", menuName = "TEBU/Content/ScriptableProduct")]
public class ScriptableProduct : ScriptableObject
{
    [Tooltip("The name of the product (e.g. Wheat, Bricks, etc.). If the product you're looking for, you might have to add it to the Products enum.")]
    public ProductType productName;

    [Tooltip("The icon representing this product in the UI.")]
    public Sprite productIcon;
}
