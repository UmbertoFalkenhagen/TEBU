using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingFactory : MonoBehaviour,IFactory<ScriptableTile>
{
    public GameObject CreateObject(ScriptableTile data, Vector3 position, Quaternion rotation, Transform parent = null)
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
