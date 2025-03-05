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
            //activeTile.SetSelected(false);
            HexMapManager.Instance.selectTile(false, activeTile);
        }

        // Setze das neue Tile
        activeTile = newTile;
        activeTileID = newTile.TileID;

        // Aktiviere das neue Tile
        if (activeTile != null)
        {
           // activeTile.SetSelected(true);
            HexMapManager.Instance.selectTile(true, activeTile);
        }
        OnActiveTileChanged?.Invoke(activeTile);
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
