using UnityEngine;
using System.Collections.Generic;

// Define a class to hold resource data, representing the resources that can spawn on a tile
[System.Serializable]
public class ResourceProbability
{
    public Resource resourceName;      // Name of the resource, e.g., "Herbs", "Raw Rice"
    public GameObject resourcePrefab; // Prefab representing the resource (e.g., a plant or mineral)
    [Range(0, 1)]
    public float spawnProbability;   // Probability of this resource spawning on the tile (0 to 1 range)
}

[CreateAssetMenu(fileName = "NewTile", menuName = "HexGrid/ScriptableTile")]
public class ScriptableTile : ScriptableObject
{
    public Resource resource;
    public TileType tileType;                        // The type of tile (e.g., Grassland, Forest)
    public GameObject prefab;                        // The visual prefab to represent this tile type itself
    public List<ResourceProbability> resources;      // List of resources and their spawn probabilities

    [Header("Default Tile Properties")]
    public GameObject defaultPrefab;                 // Default visual prefab for the tile if no resources spawn

    // Method to get the prefab that should be spawned on this tile based on probabilities
    public GameObject GetInitialTileObject()
    {
        // Iterate through the resources list to see if any resource should be spawned based on probability
        foreach (var _resource in resources)
        {
            if (Random.value <= _resource.spawnProbability)
            {
                resource = _resource.resourceName;
                return _resource.resourcePrefab;  // Return the prefab of the resource if it spawns
            }
        }

        // If no resources spawn, return the default prefab
        resource = Resource.None;
        return defaultPrefab;
    }
}
