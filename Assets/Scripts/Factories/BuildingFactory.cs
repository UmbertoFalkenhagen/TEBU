using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingFactory : MonoBehaviour
{
    public static BuildingFactory Instance;

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

    internal KeyValuePair<ObjectIdentifier, DBBuildingValue> CreateBuilding(BuildingType buildingType, SDBBuildingBlueprintValue buildingData, HexTile parentTile)
    {
        Vector3 worldPosition = parentTile.gameObject.transform.position;
        Transform parent = parentTile.gameObject.transform;
        if (buildingData == null)
        {
            Debug.LogError("BuildingFactory: buildingdatadata is null!");
            return default;
        }

        //clear resource from tile
        parentTile.ClearTileResource();

        //generate new objectidentifier for the citycenter
        ObjectIdentifier buildingID = DatabaseManager.Instance.GenerateUniqueId(ObjectType.Building);

        //instantiate buildingprefab on the tile
        GameObject buildingObject = Instantiate(buildingData.prefab, worldPosition, Quaternion.identity, parent);
        if (buildingObject == null)
        {
            Debug.LogError("BuildingFactory: Failed to instantiate citycenter prefab.");
            return default;
        }
        parentTile.heldBuilding = buildingObject;

        //ensure city center component on the gameobject
        Building buildingComponent = buildingObject.GetComponent<Building>();
        if (buildingComponent == null)  // <-- Check buildingComponent
        {
            Debug.Log("Adding Building component");
            buildingComponent = buildingObject.AddComponent<Building>();
        }
        buildingComponent.buildingID = buildingID;
        buildingComponent.buildingType = buildingType;
        ObjectIdentifier parentCC = DatabaseManager.Instance.GetTileValue(parentTile.TileID).ConstructionClaims[0];
        DBBuildingValue dBBuildingValue = new DBBuildingValue(parentTile.TileID, parentCC, buildingType, buildingObject);
        

        return new KeyValuePair<ObjectIdentifier, DBBuildingValue>(buildingID, dBBuildingValue);
    }
}
