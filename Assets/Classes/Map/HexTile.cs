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
