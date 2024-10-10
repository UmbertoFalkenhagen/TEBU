using UnityEngine;
using System.Collections.Generic;

using UnityEngine;
using System.Collections.Generic;

// Define a class to hold resource data, representing the resources that can spawn on a tile
[System.Serializable]
public class ResourceProbability
{
    [Tooltip("The type of resource that can spawn on this tile, such as 'Herbs' or 'Raw Rice'.")]
    public Resource resourceName;      // Name of the resource, e.g., "Herbs", "Raw Rice". See Enums class

    [Tooltip("The prefab used to visually represent this resource on the tile.")]
    public GameObject resourcePrefab;  // Prefab representing the resource (e.g., a plant or mineral)

    [Tooltip("Probability (0 to 1) of this resource spawning on the tile.")]
    [Range(0, 1)]
    public float spawnProbability;     // Probability of this resource spawning on the tile (0 to 1 range)
}

[CreateAssetMenu(fileName = "NewTile", menuName = "TEBU/Map/ScriptableTile")]
public class ScriptableTile : ScriptableObject
{
    [Tooltip("The resource currently associated with this tile. This is dynamically assigned during runtime.")]
    public Resource resource;         // Current resource on the tile

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

[CreateAssetMenu(fileName = "NewAnimal", menuName = "TEBU/Content/ScriptableAnimal")]
public class SciptableAnimal : ScriptableObject
{

}

[CreateAssetMenu(fileName = "NewBuilding", menuName = "TEBU/Content/ScriptableBuilding")]
public class SciptableBuilding : ScriptableObject
{

}