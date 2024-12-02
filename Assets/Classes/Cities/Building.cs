using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [Tooltip("Any resources required on selected or adjacent tiles for building construction.")]
    public List<ResourceType> requiredResources;

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

    public void Initialize(ScriptableBuilding buildingData)
    {
        requiredResources = buildingData.requiredResources;
        product = buildingData.product;
        productionPerWorker = buildingData.productionPerWorker;
        isMaxWorkersFixed = buildingData.isMaxWorkersFixed;
        inputProducts = buildingData.inputProducts;

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
