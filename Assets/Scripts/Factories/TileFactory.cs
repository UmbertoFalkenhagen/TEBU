using UnityEngine;

public class TileFactory : MonoBehaviour
{
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

    protected GameObject InstantiatePrefab(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        if (prefab == null)
        {
            Debug.LogError("Prefab is null! Cannot instantiate object.");
            return null;
        }
        return Object.Instantiate(prefab, position, rotation, parent);
    }

    /// <summary>
    /// Creates a tile object from the given ScriptableTile data and
    /// registers it in the DatabaseManager's tile dictionary.
    /// </summary>
    /// <param name="tileData">The ScriptableTile blueprint to use.</param>
    /// <param name="dbPosition">Grid coordinates for the tile (row/column).</param>
    /// <param name="worldPosition">The 3D world position where the tile should appear.</param>
    /// <param name="parent">Optional parent transform.</param>
    /// <returns>The newly instantiated tile GameObject (or null on failure).</returns>
    public GameObject CreateObject(
        ScriptableTile tileData,
        Vector2Int dbPosition,
        Vector3 worldPosition,
        Transform parent = null)
    {
        if (tileData == null)
        {
            Debug.LogError("TileData is null. Cannot create tile.");
            return null;
        }

        // 1) Generate a unique ID for this tile
        ObjectIdentifier tileID = DatabaseManager.Instance.GenerateUniqueId(ObjectType.Tile);

        // 2) Instantiate the tile prefab
        GameObject hexTileObject = InstantiatePrefab(tileData.prefab, worldPosition, Quaternion.identity, parent);
        if (hexTileObject == null)
        {
            Debug.LogError("TileFactory: Failed to instantiate tile prefab.");
            return null;
        }

        // 3) Ensure a HexTile component is present
        HexTile hexTileComponent = hexTileObject.GetComponent<HexTile>();
        if (hexTileComponent == null)
        {
            hexTileComponent = hexTileObject.AddComponent<HexTile>();
        }

        // 4) Determine which resource (if any) is spawned on this tile
        GameObject initialResourceGO = GetInitialObjectForTile(tileData, out ResourceType assignedResource);
        // Optionally place the actual resource GameObject at runtime (visual)
        if (initialResourceGO != null)
        {
            initialResourceGO.transform.SetParent(hexTileObject.transform);
            initialResourceGO.transform.localPosition = Vector3.zero; // or some offset
        }

        // 5) Create a DBTileValue to store in DatabaseManager
        //    The tile's "type" is from tileData.tileType
        //    The "resource" is assignedResource from above
        DBTileValue dbTileValue = new DBTileValue(
            dbPosition,
            hexTileObject,
            tileData.tileType,
            assignedResource
        );

        // 6) Insert the new entry into the tileDictionary
        DatabaseManager.Instance.tileDictionary[tileID] = dbTileValue;

        // 7) Store the tileID in the HexTile component (so it knows how to look itself up)
        hexTileComponent.TileID = tileID;

        Debug.Log($"TileFactory: Created new tile [{tileID}] at DB pos {dbPosition}.");

        return hexTileObject;
    }

    /// <summary>
    /// Retrieves the initial object to place on the tile based on the ScriptableTile resource probabilities.
    /// </summary>
    /// <param name="tileData">The ScriptableTile to evaluate.</param>
    /// <param name="assignedResource">Which resource was assigned for DB storage.</param>
    /// <returns>The initial GameObject resource (or null if none spawned).</returns>
    public GameObject GetInitialObjectForTile(ScriptableTile tileData, out ResourceType assignedResource)
    {
        assignedResource = ResourceType.None; // Default

        if (tileData == null)
        {
            Debug.LogError("TileData is null! Cannot determine the initial object.");
            return null;
        }

        // Probability-based resource spawning
        foreach (var resourceProbability in tileData.resources)
        {
            if (Random.value <= resourceProbability.spawnProbability)
            {
                assignedResource = resourceProbability.resourceName;
                return resourceProbability.resourcePrefab;
            }
        }
        // If no resource spawns, return the default prefab
        return tileData.defaultPrefab;
    }
}
