using UnityEngine;
using System.Collections.Generic;

public class HexTile : MonoBehaviour
{
    // The unique ID in the database for this tile
    public ObjectIdentifier TileID { get; set; }

    // If we need to keep track of an actively placed resource object,
    // we can keep that reference here. However, the resource TYPE
    // is stored in the DB. This is purely for visuals/spawned GameObject.
    public GameObject heldResource;

    // Similarly, a building's type can be stored in DB, but the actual
    // building GameObject can remain here for easy access/visuals.
    public GameObject heldBuilding;

    // Because adjacency is stored as a list of Vector2Int in DBTileValue,
    // we remove the old "adjacentTiles" list. If you want to keep references
    // to neighbor GameObjects, you can do so, but it's often enough to
    // query adjacency from the DB when needed.

    // Example: Use DB queries to get tile data
    public DBTileValue GetMyTileValue()
    {
        if (TileID == null) return null;
        // Use a typed function we created in DatabaseManager
        return DatabaseManager.Instance.GetTileValue(TileID);
    }

    // Example: a method that obtains the tile type from DB
    public TileType GetTileType()
    {
        var val = GetMyTileValue();
        if (val != null)
        {
            return val.Type; // DBTileValue.Type
        }
        return TileType.Grassland; // fallback or default
    }

    // Similarly for resource
    public ResourceType GetResourceType()
    {
        var val = GetMyTileValue();
        if (val != null)
        {
            return val.Resource; // DBTileValue.Resource
        }
        return ResourceType.None;
    }

    // Optionally adapt your resource-placing methods
    public void PlaceResourceOnTile(GameObject newObject)
    {
        if (heldResource != null)
        {
            Destroy(heldResource);
        }

        Quaternion randomYRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
        heldResource = Instantiate(newObject, transform.position, randomYRotation, this.transform);
    }

    public void ClearTileResource()
    {
        if (heldResource != null)
        {
            heldResource.SetActive(false);
        }
    }

    // Example building placement
    public void PlaceCityCenterOnTile(ScriptableCityCenter cityCenterData)
    {
        ClearTileResource();
        if (CityCenterFactory.Instance == null)
        {
            Debug.LogError("CityCenterFactory instance is null.");
            return;
        }
        heldBuilding = CityCenterFactory.Instance.CreateObject(cityCenterData, this.gameObject, Quaternion.identity, this.gameObject);
    }

    public void PlaceBuildingOnTile(ScriptableBuilding buildingData)
    {
        ClearTileResource();
        if (BuildingFactory.Instance == null)
        {
            Debug.LogError("BuildingFactory instance is null.");
            return;
        }
        heldBuilding = BuildingFactory.Instance.CreateObject(buildingData, this.gameObject, Quaternion.identity, this.gameObject);
    }

    public void RemoveHeldBuildingFromTile()
    {
        if (heldBuilding != null)
        {
            Destroy(heldBuilding);
            heldBuilding = null;
            if (heldResource != null)
            {
                heldResource.SetActive(true);
            }
        }
    }

    // If you need adjacency references, either get them from DBTileValue:
    //   var neighbors = GetMyTileValue()?.AdjacentTilesPosition;
    // or create a function to convert those positions into actual GameObjects
    // by looking up each neighbor in the dictionary and retrieving its GameObject.
}
