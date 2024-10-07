using UnityEngine;
using System.Collections.Generic;

// Define a class to hold resource data
[System.Serializable]
public class ResourceProbability
{
    public string resourceName;      // Name of the resource, e.g., "Herbs", "Raw Rice"
    [Range(0, 1)]
    public float spawnProbability;   // Probability of this resource spawning on the tile (0 to 1 range)
}

[CreateAssetMenu(fileName = "NewTile", menuName = "HexGrid/ScriptableTile")]
public class ScriptableTile : ScriptableObject
{
    public TileType tileType;                    // The type of tile (e.g., Grassland, Forest)
    public GameObject prefab;                    // The visual prefab to represent this tile type
    public List<ResourceProbability> resources;  // List of resources and their spawn probabilities
}
