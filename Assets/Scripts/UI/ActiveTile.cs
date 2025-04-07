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

    public static event Action<ObjectIdentifier> OnActiveTileChanged;
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
            activeTile.SelectTile(false);
        }

        // Setze das neue Tile
        activeTile = newTile;
        activeTileID = newTile.TileID;

        // Aktiviere das neue Tile
        if (activeTile != null)
        {
            activeTile.SelectTile(true);
        }
        OnActiveTileChanged?.Invoke(activeTileID);
    }

    // Gibt das aktuell aktive Tile zurück
    public ObjectIdentifier GetActiveTileID()
    {
        return activeTileID;
    }
    public HexTile GetActiveTile()
    {
        return activeTile;
    }
    // Gibt das zuletzt aktive Tile zurück
    public ObjectIdentifier GetLastActiveTile()
    {
        return lastActiveTileID;
    }
}
