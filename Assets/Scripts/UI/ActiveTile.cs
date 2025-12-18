using UnityEngine;
using System;

public class ActiveTile : MonoBehaviour
{
    #region Singleton

    public static ActiveTile Instance { get; private set; }

    #endregion

    #region State

    private HexTile activeTile;
    private HexTile lastActiveTile;
    private ObjectIdentifier activeTileID;
    private ObjectIdentifier lastActiveTileID;

    #endregion

    #region Events

    public static event Action<ObjectIdentifier> OnActiveTileChanged;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #endregion

    #region Public Methods

    public void SetActiveTile(HexTile newTile)
    {
        if (activeTile == newTile || newTile == null) return;

        if (activeTile != null)
        {
            lastActiveTile = activeTile;
            lastActiveTileID = activeTileID;
            activeTile.SelectTile(false);
        }

        activeTile = newTile;
        activeTileID = newTile.TileID;

        if (activeTile != null)
        {
            activeTile.SelectTile(true);
        }

        OnActiveTileChanged?.Invoke(activeTileID);
    }

    public ObjectIdentifier GetActiveTileID()
    {
        return activeTileID;
    }

    public HexTile GetActiveTile()
    {
        return activeTile;
    }

    public ObjectIdentifier GetLastActiveTileID()
    {
        return lastActiveTileID;
    }

    public HexTile GetLastActiveTile()
    {
        return lastActiveTile;
    }

    #endregion
}
