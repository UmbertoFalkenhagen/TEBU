using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceBar : MonoBehaviour
{
    public static ResourceBar Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void UpdateResourceBar(ObjectIdentifier cityID, DatabaseManager database)
    {
        if (cityID!= null && cityID.ToString().Contains("#"))
        {


        }
        else
        {
            Debug.Log("Not a City ID");
        }
    }
}
