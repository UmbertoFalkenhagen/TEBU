using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityCenterManager : MonoBehaviour
{
    public static CityCenterManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Build() //return something to tell if all went good
    {
        //build citycenter
        //add to database
        HexTile tileToBuildOn = ActiveTile.Instance.GetActiveTile();
        ScriptableCityCenter sCC = Resources.Load<ScriptableCityCenter>("Data/Buildings/CityCenter_Grassland");
        KeyValuePair<ObjectIdentifier, DBCityCenterValue> citycenter = CityCenterFactory.Instance.CreateCityCenter(sCC, tileToBuildOn);

        if (citycenter.Value != null)
        {
            // e.g. random tile
            DatabaseManager.Instance.AddCityCenter(citycenter.Key, citycenter.Value);
            CreateConstructionClaimsForCityCenter(citycenter.Key);
        }
        UIManager.Instance.tileClick(ActiveTile.Instance.GetActiveTile());
    }

    public void CreateConstructionClaimsForCityCenter(ObjectIdentifier cityCenterId)
    {
        // 1) Validate that the passed identifier is for a CityCenter
        if (cityCenterId.Type != ObjectType.CityCenter)
        {
            Debug.LogError($"CreateConstructionClaimsForCityCenter: Provided ID [{cityCenterId}] is not a CityCenter.");
            return;
        }

        // 2) Use DatabaseManager to get the parent tile of this CityCenter
        ObjectIdentifier parentTileId = DatabaseManager.Instance.GetParentTileIdByCityCenterId(cityCenterId);
        if (parentTileId == null)
        {
            Debug.LogError($"CreateConstructionClaimsForCityCenter: CityCenter [{cityCenterId}] has no valid parent tile.");
            return;
        }

        // 3) Retrieve all adjacent tiles around that parent tile
        List<DBTileValue> adjacentTiles = DatabaseManager.Instance.GetTileNeighbors(parentTileId);
        if (adjacentTiles == null || adjacentTiles.Count == 0)
        {
            Debug.Log($"CreateConstructionClaimsForCityCenter: No adjacent tiles found for CityCenter [{cityCenterId}].");
            return;
        }

        // 4) Add the CityCenter’s ObjectIdentifier to each tile’s construction claims list
        foreach (DBTileValue neighborTile in adjacentTiles)
        {
            // a) Ensure DBTileValue has a List<ObjectIdentifier> constructionClaims
            if (!neighborTile.ConstructionClaims.Contains(cityCenterId))
            {
                neighborTile.ConstructionClaims.Add(cityCenterId);
            }

            /*// b) Check whether this cityCenterId is the FIRST claimant
            Renderer tileRenderer = neighborTile.TileObject != null
                ? neighborTile.TileObject.GetComponent<Renderer>()
                : null;

            if (tileRenderer != null && neighborTile.ConstructionClaims.Count > 0)
            {
                if (neighborTile.ConstructionClaims[0] == cityCenterId)
                {
                    // If our CityCenter is the first claimant, set tile color to red
                    tileRenderer.material.color = Color.red;
                }
                else
                {
                    // If there's already another claimant, set tile color to yellow
                    tileRenderer.material.color = Color.yellow;
                }
            }*/
        }

        Debug.Log($"CreateConstructionClaimsForCityCenter: Added CityCenter [{cityCenterId}] as a claimant to {adjacentTiles.Count} tiles.");
    }

}
