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

    public void Build()
    {
        HexTile tileToBuildOn = ActiveTile.Instance.GetActiveTile();
        ScriptableCityCenter sCC = Resources.Load<ScriptableCityCenter>("Data/Buildings/CityCenter_Grassland");
        KeyValuePair<ObjectIdentifier, DBCityCenterValue> citycenter = CityCenterFactory.Instance.CreateCityCenter(sCC, tileToBuildOn);

        if (citycenter.Value != null)
        {
            DatabaseManager.Instance.AddCityCenter(citycenter.Key, citycenter.Value);
            CreateConstructionClaimsForCityCenter(citycenter.Key);

            SpawnInitialAnimal(citycenter.Key);
        }
        UIManager.Instance.tileClick(ActiveTile.Instance.GetActiveTileID());
    }

    private void SpawnInitialAnimal(ObjectIdentifier cityCenterId)
    {
        if (AnimalManager.Instance == null)
        {
            Debug.LogWarning("AnimalManager.Instance is null. Make sure AnimalManager GameObject exists in the scene.");
            return;
        }

        Animal spawnedAnimal = AnimalManager.Instance.SpawnRandomAnimalForCity(cityCenterId);

        if (spawnedAnimal != null)
        {
            spawnedAnimal.SayHello();
        }
        else
        {
            Debug.LogWarning($"Failed to spawn initial animal for city {cityCenterId}");
        }
    }

    public void CreateConstructionClaimsForCityCenter(ObjectIdentifier cityCenterId)
    {
        if (cityCenterId.Type != ObjectType.CityCenter)
        {
            Debug.LogError($"CreateConstructionClaimsForCityCenter: Provided ID [{cityCenterId}] is not a CityCenter.");
            return;
        }

        ObjectIdentifier parentTileId = DatabaseManager.Instance.GetParentTileIdByCityCenterId(cityCenterId);
        if (parentTileId == null)
        {
            Debug.LogError($"CreateConstructionClaimsForCityCenter: CityCenter [{cityCenterId}] has no valid parent tile.");
            return;
        }

        List<DBTileValue> adjacentTiles = DatabaseManager.Instance.GetTileNeighbors(parentTileId);
        if (adjacentTiles == null || adjacentTiles.Count == 0)
        {
            Debug.Log($"CreateConstructionClaimsForCityCenter: No adjacent tiles found for CityCenter [{cityCenterId}].");
            return;
        }

        foreach (DBTileValue neighborTile in adjacentTiles)
        {
            if (!neighborTile.ConstructionClaims.Contains(cityCenterId))
            {
                neighborTile.TileObject.GetComponent<HexTile>().constructionClaims.Add(cityCenterId);
                neighborTile.ConstructionClaims.Add(cityCenterId);
            }
        }

        Debug.Log($"CreateConstructionClaimsForCityCenter: Added CityCenter [{cityCenterId}] as a claimant to {adjacentTiles.Count} tiles.");
    }
}
