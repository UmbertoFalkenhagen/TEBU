using UnityEngine;

public class HexTile : MonoBehaviour
{
    public Vector2Int HexCoords;    // The coordinates of this hex in the grid
    public TileType TileType;         // Type of tile (e.g., "Grassland", "Forest", etc.)
    public GameObject heldObject;   // The GameObject currently held on this tile (resource, building, etc.)
    public Resource resource;

    // Method to place an object on the tile, replacing any existing one
    public void PlaceResourceOnTile(GameObject newObject)
    {
        // Remove the existing object if there is one
        if (heldObject != null)
        {
            Destroy(heldObject);
        }

        // Set a random rotation around the Y axis for a more natural look
        Quaternion randomYRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

        // Set the new object as the held object and instantiate it on the tile
        heldObject = Instantiate(newObject, transform.position, randomYRotation, this.transform);
    }

    // Method to clear the tile of any held object
    public void ClearTile()
    {
        if (heldObject != null)
        {
            Destroy(heldObject);
            heldObject = null;
        }
    }
}

