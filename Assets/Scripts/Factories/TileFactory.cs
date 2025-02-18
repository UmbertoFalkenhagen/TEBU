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
    public void AddRandomTile(Vector2Int dbPosition, Vector3 worldPosition, Transform parent = null)
    {
        // Make sure we have tileBlueprints in the DB
        var manager = DatabaseManager.Instance;
        if (manager.tileBlueprints == null || manager.tileBlueprints.Count == 0)
        {
            Debug.LogError("TileFactory: No ScriptableTiles found in DatabaseManager.tileBlueprints!");
            return;
        }
        int randomIndex = Random.Range(0, manager.tileBlueprints.Count);
        ScriptableTile chosenTile = manager.tileBlueprints[randomIndex];

        CreateTile(chosenTile, dbPosition, worldPosition, parent);
    }

    // AddTileOfType
    public void AddTileOfType(TileType tileType, Vector2Int dbPosition, Vector3 worldPosition, Transform parent = null)
    {
        // Find the scriptable tile with matching tileType
        ScriptableTile tileData = DatabaseManager.Instance.tileBlueprints.FirstOrDefault(t => t.tileType == tileType);
        if (tileData == null)
        {
            Debug.LogWarning($"TileFactory: No ScriptableTile found for TileType '{tileType}'");
            return;
        }

        CreateTile(tileData, dbPosition, worldPosition, parent);
    }

    // Core creation logic - used by both methods
    private void CreateTile(ScriptableTile tileData, Vector2Int dbPosition, Vector3 worldPosition, Transform parent)
    {
        if (tileData == null)
        {
            Debug.LogError("TileFactory: tileData is null!");
            return;
        }

        // 1) Generate a unique ID (ObjectType.Tile)
        ObjectIdentifier tileID = DatabaseManager.Instance.GenerateUniqueId(ObjectType.Tile);

        // 2) Instantiate the prefab
        GameObject hexTileObject = InstantiatePrefab(tileData.prefab, worldPosition, Quaternion.identity, parent);
        if (hexTileObject == null)
        {
            Debug.LogError("TileFactory: Failed to instantiate tile prefab.");
            return;
        }

        // 3) Ensure HexTile component
        HexTile hexTile = hexTileObject.GetComponent<HexTile>();
        if (hexTile == null)
        {
            hexTile = hexTileObject.AddComponent<HexTile>();
        }

        // 4) Possibly set up resource logic
        GameObject initialResource = GetInitialObjectForTile(tileData, out ResourceType assignedResource);
        if (initialResource != null)
        {
            initialResource.transform.SetParent(hexTileObject.transform);
            initialResource.transform.localPosition = Vector3.zero;
        }

        // 5) Create DBTileValue
        DBTileValue tileValue = new DBTileValue(dbPosition, hexTileObject, tileData.tileType, assignedResource);

        // 6) Add new entry to the dictionary
        DatabaseManager.Instance.AddTile(tileID, tileValue);

        // 7) Let the HexTile know its ID
        hexTile.TileID = tileID;
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
    private GameObject GetInitialObjectForTile(ScriptableTile tileData, out ResourceType assignedResource)
    {
        assignedResource = ResourceType.None;
        if (tileData == null) return null;

        foreach (var resourceProbability in tileData.resources)
        {
            if (Random.value <= resourceProbability.spawnProbability)
            {
                assignedResource = resourceProbability.resourceName;
                return resourceProbability.resourcePrefab;
            }
        }
        // fallback to default prefab
        return tileData.defaultPrefab;
    }
}
