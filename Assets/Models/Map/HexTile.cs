using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexTile : MonoBehaviour
{
    public Vector2Int HexCoords;  // The coordinates of this hex in the grid
    public string TileType;       // e.g., "Grassland", "Forest", etc.
    public GameObject Structure;  // The building or resource on this tile
}
