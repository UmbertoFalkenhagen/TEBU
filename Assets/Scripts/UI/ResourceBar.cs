using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceBar : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
