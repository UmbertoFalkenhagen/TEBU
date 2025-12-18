using UnityEngine;
using System.Collections.Generic;

public class HexTile : MonoBehaviour
{
    #region Database Reference

    public ObjectIdentifier TileID;

    #endregion

    #region Visual References

    public GameObject heldResource;
    public GameObject heldBuilding;

    #endregion

    #region Claim Lists

    public List<ObjectIdentifier> constructionClaims = new List<ObjectIdentifier>();
    public List<ObjectIdentifier> activeClaims = new List<ObjectIdentifier>();

    #endregion

    #region Database Query Helpers

    public DBTileValue GetMyTileValue()
    {
        if (TileID == null) return null;
        return DatabaseManager.Instance.GetTileValue(TileID);
    }

    public ResourceType GetResourceType()
    {
        var val = GetMyTileValue();
        if (val != null)
        {
            return val.Resource;
        }
        return ResourceType.None;
    }

    public TileType GetTileType()
    {
        var val = GetMyTileValue();
        if (val != null)
        {
            return val.Type;
        }
        return TileType.Grassland;
    }

    #endregion

    #region Visual Object Management

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

    public void ClearTileBuilding()
    {
        if (heldBuilding != null)
        {
            Destroy(heldBuilding);
            heldBuilding = null;
        }
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

    public void SelectTile(bool isSelected)
    {
        if (isSelected)
        {
            this.transform.position += new Vector3(0, 1, 0);
        }
        else
        {
            this.transform.position -= new Vector3(0, 1, 0);
        }
    }

    #endregion
}
