using UnityEngine;
using System.Collections.Generic;

// Define a class to hold resource data, representing the resources that can spawn on a tile
[System.Serializable]
public class ResourceProbability
{
    [Tooltip("The type of resource that can spawn on this tile, such as 'Herbs' or 'Raw Rice'.")]
    public ResourceType resourceName;      // Name of the resource, e.g., "Herbs", "Raw Rice". See Enums class

    [Tooltip("The prefab used to visually represent this resource on the tile.")]
    public GameObject resourcePrefab;  // Prefab representing the resource (e.g., a plant or mineral)

    [Tooltip("Probability (0 to 1) of this resource spawning on the tile.")]
    [Range(0, 1)]
    public float spawnProbability;     // Probability of this resource spawning on the tile (0 to 1 range)
}

[System.Serializable]
public class ProductRequirement
{
    [Tooltip("The type of product required for production.")]
    public ProductType product;

    [Tooltip("The quantity of the product required for production.")]
    public int quantity;
}

[CreateAssetMenu(fileName = "NewTile", menuName = "TEBU/Map/ScriptableTile")]
public class ScriptableTile : ScriptableObject
{
    [Tooltip("The type of this tile, such as Grassland, Forest, Desert, etc.")]
    public TileType tileType;         // The type of tile (e.g., Grassland, Forest)

    [Tooltip("The visual prefab used to represent this tile type.")]
    public GameObject prefab;         // The visual prefab to represent this tile type itself

    [Tooltip("A list of resources that can potentially spawn on this tile along with their spawn probabilities.")]
    public List<ResourceProbability> resources;  // List of resources and their spawn probabilities

    [Header("Default Tile Properties")]
    [Tooltip("The default visual prefab for this tile if no resources are spawned. For example, this could be a patch of grass or barren ground.")]
    public GameObject defaultPrefab;  // Default visual prefab for the tile if no resources spawn
}

[CreateAssetMenu(fileName = "NewScriptableResource", menuName = "TEBU/Map/ScriptableResource")]
public class ScriptableResource : ScriptableObject
{
    [Tooltip("The resource type associated with this Scriptable Resource (e.g., Rice, Herbs, etc.). If the resource you're looking for, you might have to add it to the Resources enum.")]
    public ResourceType resourcename;

    [Tooltip("The prefab representing this resource in the game world.")]
    public GameObject resourcePrefab;
}

[CreateAssetMenu(fileName = "NewScriptableProduct", menuName = "TEBU/Content/ScriptableProduct")]
public class ScriptableProduct : ScriptableObject
{
    [Tooltip("The name of the product (e.g. Wheat, Bricks, etc.). If the product you're looking for, you might have to add it to the Products enum.")]
    public ProductType productName;

    [Tooltip("The icon representing this product in the UI.")]
    public Sprite productIcon;
}

[CreateAssetMenu(fileName = "NewScriptableAnimal", menuName = "TEBU/Content/ScriptableAnimal")]
public class ScriptableAnimal : ScriptableObject
{
    [Tooltip("The name of the animal (e.g., Cow, Sheep, Beaver, etc.). If the animal you're looking for, you might have to add it to the Animals enum.")]
    public AnimalType animalName;

    [Tooltip("The prefab representing this animal in the game world.")]
    public GameObject prefab;

    [Tooltip("The tile type where this animal can spawn (e.g., Grassland, Forest).")]
    public TileType spawnLocation;

    [Tooltip("The basic food product required for this animal to start spawning in a city.")]
    public ProductType basicFood;

    [Header("Basic Ability Unlocks")]
    [Tooltip("The product required to unlock the first basic ability of this animal.")]
    public ProductType basicAbilityUnlockProduct1;

    [Tooltip("The product required to unlock the second basic ability of this animal.")]
    public ProductType basicAbilityUnlockProduct2;

    [Header("Ability Improvements")]
    [Tooltip("The product required to improve the first ability.")]
    public ProductType abilityImprovementProduct1;

    [Tooltip("The product required to improve the second ability.")]
    public ProductType abilityImprovementProduct2;

    // Placeholder for future ability system
    // public ScriptableAbility[] abilities;
}

[CreateAssetMenu(fileName = "NewScriptableBuilding", menuName = "TEBU/Content/ScriptableBuilding")]
public class ScriptableBuilding : ScriptableObject
{
    [Tooltip("The type of building (e.g., WheatFarm, Brickhut, etc.). If the building you're looking for, you might have to add it to the Buildings enum.")]
    public BuildingType buildingName;

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

[CreateAssetMenu(fileName = "NewScriptableCityCenter", menuName = "TEBU/Content/ScriptableCityCenter")]
public class ScriptableCityCenter : ScriptableObject
{
    [Tooltip("The tile type where this city center can be constructed (e.g., Grassland, Plains).")]
    public TileType cityLocation;

    [Tooltip("The prefab representing the initial version of the city center.")]
    public GameObject basicPrefab;

    [Tooltip("The prefab representing the upgraded version of the city center.")]
    public GameObject upgradedPrefab;
}