using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class TileFactory : MonoBehaviour
{
    public static TileFactory Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // AddRandomTile
    public KeyValuePair<ObjectIdentifier, DBTileValue> AddRandomTile(Vector2Int dbPosition, Vector3 worldPosition, Transform parent = null)
    {
        // Make sure we have tileBlueprints in the DB
        var manager = DatabaseManager.Instance;
        if (manager.tileBlueprints == null || manager.tileBlueprints.Count == 0)
        {
            Debug.LogError("TileFactory: No ScriptableTiles found in DatabaseManager.tileBlueprints!");
            return default;
        }
        int randomIndex = Random.Range(0, manager.tileBlueprints.Count);
        ScriptableTile chosenTile = manager.tileBlueprints[randomIndex];

        return CreateTile(chosenTile, dbPosition, worldPosition, parent);
    }

    // AddTileOfType
    public KeyValuePair<ObjectIdentifier, DBTileValue> AddTileOfType(TileType tileType, Vector2Int dbPosition, Vector3 worldPosition, Transform parent = null)
    {
        // Find the scriptable tile with matching tileType
        ScriptableTile tileData = DatabaseManager.Instance.tileBlueprints.FirstOrDefault(t => t.tileType == tileType);
        if (tileData == null)
        {
            Debug.LogWarning($"TileFactory: No ScriptableTile found for TileType '{tileType}'");
            return default;
        }

        return CreateTile(tileData, dbPosition, worldPosition, parent);
    }

    // Core creation logic - used by both methods
    private KeyValuePair<ObjectIdentifier, DBTileValue> CreateTile(ScriptableTile tileData, Vector2Int dbPosition, Vector3 worldPosition, Transform parent)
    {
        if (tileData == null)
        {
            Debug.LogError("TileFactory: tileData is null!");
            return default;
        }

        // 1) Generate a unique ID (ObjectType.Tile)
        ObjectIdentifier tileID = DatabaseManager.Instance.GenerateUniqueId(ObjectType.Tile);

        // 2) Instantiate the prefab
        GameObject hexTileObject = InstantiatePrefab(tileData.prefab, worldPosition, Quaternion.identity, parent);
        if (hexTileObject == null)
        {
            Debug.LogError("TileFactory: Failed to instantiate tile prefab.");
            return default;
        }

        // 3) Ensure HexTile component
        HexTile hexTile = hexTileObject.GetComponent<HexTile>();
        if (hexTile == null)
        {
            hexTile = hexTileObject.AddComponent<HexTile>();
        }

        // Make HexTile clickable
        hexTileObject.layer = LayerMask.NameToLayer("Tile");
        // hexTileObject.AddComponent<ObjectID>().objectID = tileID;

        ResourceType assignedResource;
        GetInitialObjectForTile(tileData, out assignedResource);

        GameObject resourceInstance = null;

        if (assignedResource != ResourceType.None)
        {
            // Get resource prefab from DatabaseManager
            var resourcePrefab = DatabaseManager.Instance.GetResourcePrefabForType(assignedResource);
            if (resourcePrefab != null)
            {
                // Create random Y rotation
                float randomYRotation = Random.Range(0f, 360f);
                Quaternion randomRotation = Quaternion.Euler(0f, randomYRotation, 0f);

                // Instantiate resource as child of the tile
                Vector3 spawnPos = worldPosition; // You can offset if needed
                resourceInstance = InstantiatePrefab(resourcePrefab, spawnPos, randomRotation, hexTileObject.transform);
            }
            else
            {
                Debug.LogWarning($"TileFactory: No prefab found for ResourceType '{assignedResource}'");
            }
        }

        // 5) Create DBTileValue
        DBTileValue tileValue = new DBTileValue(dbPosition, hexTileObject, tileData.tileType, assignedResource);

        // 7) Let the HexTile know its ID
        hexTile.TileID = tileID;

        return new KeyValuePair<ObjectIdentifier, DBTileValue>(tileID, tileValue);
    }

    // Instantiate Prefab (copied from old code)
    private GameObject InstantiatePrefab(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
    {
        if (prefab == null)
        {
            Debug.LogError("Prefab is null! Cannot instantiate object.");
            return null;
        }
        return Instantiate(prefab, position, rotation, parent);
    }

    // Probability-based resource spawning
    private ResourceType GetInitialObjectForTile(ScriptableTile tileData, out ResourceType assignedResource)
    {
        assignedResource = tileData.defaultResource != null ? tileData.defaultResource : ResourceType.None;


        foreach (var resourceProbability in tileData.resources)
        {
            if (Random.value <= resourceProbability.spawnProbability)
            {
                assignedResource = resourceProbability.resourceName;
                return assignedResource;
                
            }
        }
        // fallback to default prefab
        return assignedResource;
    }
}
