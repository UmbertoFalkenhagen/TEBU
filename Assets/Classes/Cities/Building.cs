using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    [Tooltip("The selection of tile types on which the building can be constructed.")]
    public List<TileType> suitableTileTypeLocations;

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
    public List<GameObject> workableTiles;

    public void Initialize(ScriptableBuilding buildingData)
    {
        suitableTileTypeLocations = buildingData.suitableTileTypeLocations;
        requiredResources = buildingData.requiredResources;
        product = buildingData.product;
        productionPerWorker = buildingData.productionPerWorker;
        isMaxWorkersFixed = buildingData.isMaxWorkersFixed;
        inputProducts = buildingData.inputProducts;

    }
    // Start is called before the first frame update
    void Start()
    {
        if (!isMaxWorkersFixed)
        {
            CheckForWorkableTiles();
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckForWorkableTiles()
    {
        List<GameObject> tiles = GridMap.Instance.FindTilesAtEdgeDistance(this.transform.parent.gameObject, 1f);
        Debug.Log("Found " + tiles.Count + " tiles at distance.");
        workableTiles = new List<GameObject>();
        workableTiles.Add(this.transform.parent.gameObject);
        foreach (var tile in tiles)
        {
            HexTile hexTileComponent = tile.GetComponent<HexTile>();
            if (requiredResources[0] != ResourceType.None)
            {
                if (requiredResources.Contains(hexTileComponent.resource) && hexTileComponent.heldBuilding == null && suitableTileTypeLocations.Contains(hexTileComponent.TileType))
                {
                    workableTiles.Add(tile);
                }

            } else if (requiredResources[0] == ResourceType.None) {
                if (hexTileComponent.heldBuilding == null && suitableTileTypeLocations.Contains(hexTileComponent.TileType))
                {
                    workableTiles.Add(tile);
                }
            }
            
        }

        maxWorkers = workableTiles.Count;
    }
}
