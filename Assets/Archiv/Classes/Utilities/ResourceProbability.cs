using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
