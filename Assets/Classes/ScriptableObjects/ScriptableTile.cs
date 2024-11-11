using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
