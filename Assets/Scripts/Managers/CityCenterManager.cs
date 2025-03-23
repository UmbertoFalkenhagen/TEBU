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
        }
        UIManager.Instance.tileClick(ActiveTile.Instance.GetActiveTile());
    }
}
