using UnityEngine;

/// <summary>
/// Factory class for creating tile GameObjects from ScriptableTile data.
/// </summary>
public class TileFactory : MonoBehaviour, IFactory<ScriptableTile>
{
    // Singleton pattern for global access (optional)
    public static TileFactory Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            Debug.LogWarning("Duplicate TileFactory instance destroyed.");
        }
    }

    /// <summary>
    /// Creates a tile GameObject using the provided ScriptableTile data.
    /// </summary>
    public GameObject CreateElement(ScriptableTile tileData, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (tileData == null)
        {
            Debug.LogError("TileData is null. Cannot create tile.");
            return null;
        }

        // Instantiate the tile prefab at the specified position and rotation
        GameObject hexTileObject = Instantiate(tileData.prefab, position, rotation, parent);

        // Set up the HexTile component with the chosen properties
        HexTile hexTileComponent = hexTileObject.GetComponent<HexTile>();
        if (hexTileComponent != null)
        {
            hexTileComponent.TileType = tileData.tileType;  // Set the tile type from the ScriptableTile
            hexTileComponent.resource = tileData.resource;
        }
        else
        {
            Debug.LogError($"HexTile component missing on the instantiated prefab.");
        }

        return hexTileObject;
    }

    /// <summary>
    /// Retrieves the initial object to place on the tile based on the ScriptableTile resource probabilities.
    /// </summary>
    public GameObject GetInitialObjectForTile(ScriptableTile tileData)
    {
        if (tileData == null)
        {
            Debug.LogError("TileData is null! Cannot determine the initial object.");
            return null;
        }

        // Iterate through the resources list to see if any resource should be spawned based on probability
        foreach (var resourceProbability in tileData.resources)
        {
            if (Random.value <= resourceProbability.spawnProbability)
            {
                return resourceProbability.resourcePrefab; // Return the prefab of the resource if it spawns
            }
        }

        // If no resources spawn, return the default prefab
        return tileData.defaultPrefab;
    }
}
