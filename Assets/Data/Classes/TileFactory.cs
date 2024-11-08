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

    // CreateInstance method
    protected GameObject InstantiatePrefab(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (prefab == null)
        {
            Debug.LogError("Prefab is null! Cannot instantiate object.");
            return null;
        }
        return Object.Instantiate(prefab, position, rotation, parent);
    }

    public GameObject CreateObject(ScriptableTile tileData, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (tileData == null)
        {
            Debug.LogError("TileData is null. Cannot create tile.");
            return null;
        }

        // Instantiate the tile prefab at the specified position and rotation
        GameObject hexTileObject = InstantiatePrefab(tileData.prefab, position, rotation, parent);

        HexTile hexTileComponent = hexTileObject.GetComponent<HexTile>();
        if (hexTileComponent == null) {

            // Set up the HexTile component with the chosen properties
            hexTileComponent = hexTileObject.AddComponent<HexTile>();
        }

        TileClick tileClickComponent = hexTileObject.GetComponent<TileClick>();
        if (tileClickComponent == null)
        {
            //Add TileClick script
            tileClickComponent = hexTileObject.AddComponent<TileClick>();


        }

        //add tile to layer "Tile"
        hexTileObject.layer = 6;
        if (hexTileComponent != null)
        {
            hexTileComponent.TileType = tileData.tileType;  // Set the tile type from the ScriptableTile

            // Get the initial object to place on the tile and the assigned resource
            GameObject initialResource = GetInitialObjectForTile(tileData, out ResourceType assignedResource);

            // Store the resource in the HexTile component
            hexTileComponent.resource = assignedResource;

            // Place the initial object on the tile
            if (initialResource != null)
            {
                hexTileComponent.PlaceResourceOnTile(initialResource);
            }
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
    /// <param name="tileData">The ScriptableTile to evaluate.</param>
    /// <param name="out Resource">The determined resource to assign to the HexTile.</param>
    /// <returns>The initial GameObject to be placed on the tile.</returns>
    public GameObject GetInitialObjectForTile(ScriptableTile tileData, out ResourceType assignedResource)
    {
        assignedResource = ResourceType.None; // Default to None

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
                assignedResource = resourceProbability.resourceName; // Assign the resource
                return resourceProbability.resourcePrefab; // Return the prefab of the resource if it spawns
            }
        }

        // If no resources spawn, return the default prefab
        return tileData.defaultPrefab;
    }
}
