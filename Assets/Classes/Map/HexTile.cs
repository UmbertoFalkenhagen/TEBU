using UnityEngine;

public class HexTile : MonoBehaviour
{
    public Vector2Int HexCoords; // Coordinates in the grid
    public TileType TileType;    // Type of tile (e.g., Grassland, Forest, etc.)
    public GameObject heldObject;
    public ResourceType resource;

    public void PlaceResourceOnTile(GameObject newObject)
    {
        if (heldObject != null)
        {
            Destroy(heldObject);
        }

        Quaternion randomYRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
        heldObject = Instantiate(newObject, transform.position, randomYRotation, this.transform);
    }

    public void ClearTile()
    {
        if (heldObject != null)
        {
            Destroy(heldObject);
            heldObject = null;
        }
    }

    // Returns the world position of this tile's GameObject
    public Vector3 GetTileCoordinates()
    {
        return transform.position;
    }
}
