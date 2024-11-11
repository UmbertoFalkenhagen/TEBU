using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewScriptableBuilding", menuName = "TEBU/Content/ScriptableBuilding")]
public class ScriptableBuilding : ScriptableObject
{
    [Tooltip("The type of building (e.g., WheatFarm, Brickhut, etc.). If the building you're looking for, you might have to add it to the Buildings enum.")]
    public BuildingType buildingName;

    [Tooltip("The prefab representing the initial version of the building.")]
    public GameObject basicPrefab;

    [Tooltip("The type of product this building produces.")]
    public ProductType product;

    [Tooltip("The amount of product produced by each worker.")]
    public int productionPerWorker;

    [Tooltip("Determines if the number of workers is fixed or dynamic.")]
    public bool isMaxWorkersFixed;

    [Tooltip("The maximum number of workers this building can have (only relevant if max workers is fixed).")]
    public int maxWorkers;

    [Header("Input Products for Production")]
    [Tooltip("List of products and their quantities required to produce the output product.")]
    public List<ProductRequirement> inputProducts = new List<ProductRequirement>();
}
