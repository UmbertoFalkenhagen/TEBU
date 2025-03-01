using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ActiveTile : MonoBehaviour
{
    public static ActiveTile Instance { get; private set; }

    private HexTile activeTile;
    private HexTile lastActiveTile;
    private ObjectIdentifier activeTileID;
    private ObjectIdentifier lastActiveTileID;

    public static event Action<HexTile> OnActiveTileChanged;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetActiveTile(HexTile newTile)
    {
        if (activeTile == newTile || newTile == null) return; // Kein Wechsel oder null vermeiden

        // Speichere das vorherige Tile
        if (activeTile != null)
        {
            lastActiveTile = activeTile;
            lastActiveTileID = activeTileID;
            // MoveTileDown(lastActiveTile); // Altes Tile zurücksetzen
            activeTile.SetSelected(false);
        }

        // Setze das neue Tile
        activeTile = newTile;
        activeTileID = newTile.TileID;

        // Aktiviere das neue Tile
        if (activeTile != null)
        {
            activeTile.SetSelected(true);
            // MoveTileUp(activeTile);
        }
        OnActiveTileChanged?.Invoke(activeTile);
    }

    // Gibt das aktuell aktive Tile zurück
    public ObjectIdentifier GetActiveTile()
    {
        return activeTileID;
    }

    // Gibt das zuletzt aktive Tile zurück
    public ObjectIdentifier GetLastActiveTile()
    {
        return lastActiveTileID;
    }

    // Hebt das Tile um 1 Einheit an
    private void MoveTileUp(HexTile tile)
    {
        if (tile != null)
        {
            tile.transform.position += new Vector3(0, 1, 0);
        }
    }

    // Senkt das vorherige Tile wieder ab
    private void MoveTileDown(HexTile tile)
    {
        if (tile != null)
        {
            tile.transform.position -= new Vector3(0, 1, 0);
        }
    }
}
