using UnityEngine;
using System.Collections.Generic;

public class HexTile : MonoBehaviour
{
    public Vector2Int HexCoords; // Coordinates in the grid
    public TileType TileType;    // Type of tile (e.g., Grassland, Forest, etc.)
    public GameObject heldResource;
    public GameObject heldBuilding;
    public ResourceType resource;
    public List<GameObject> adjacentTiles = new List<GameObject>(); // Initialize adjacentTiles


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
            //heldResource = null;
        }
    }

    // Method to place a city center on this tile using CityCenterFactory
    public void PlaceCityCenterOnTile(ScriptableCityCenter cityCenterData)
    {
        // Deactivate any existing resource
        ClearTileResource();

        // Ensure the CityCenterFactory instance exists
        if (CityCenterFactory.Instance == null)
        {
            Debug.LogError("CityCenterFactory instance is null. Make sure CityCenterFactory is instantiated correctly.");
            return;
        }

        // Use CityCenterFactory to create the city center on this tile
        heldBuilding = CityCenterFactory.Instance.CreateObject(cityCenterData, this.gameObject, Quaternion.identity, this.gameObject);
        if (heldBuilding == null)
        {
            Debug.LogError("Failed to instantiate city center using CityCenterFactory.");
        }
    }

    // Placeholder for placing a generic building on the tile
    public void PlaceBuildingOnTile(ScriptableBuilding buildingData)
    {
        // Empty for now, will be implemented later
    }

    // Method to remove the held building from the tile
    public void RemoveHeldBuildingFromTile()
    {
        if (heldBuilding != null)
        {
            Destroy(heldBuilding);
            heldBuilding = null;

            // Reactivate any previously held resource
            if (heldResource != null)
            {
                heldResource.SetActive(true);
            }
        }
    }

    // Returns the world position of this tile's GameObject
    public Vector3 GetTileCoordinates()
    {
        return transform.position;
    }

    public void FindNeighbors(float range)
    {
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, range);
        foreach (Collider collider3D in colliderArray)
        {
            HexTile tile = collider3D.GetComponentInParent<HexTile>();
            if (tile != null && tile != this)
            {
                adjacentTiles.Add(tile.gameObject);
            }
        }
    }
}
