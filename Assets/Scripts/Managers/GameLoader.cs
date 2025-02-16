using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoader : MonoBehaviour
{

    private HexMapGenerator hexMapGenerator;
    // Start is called before the first frame update
    void Start()
    {
        //fillBlueprintDB



        //Run Generate Map
        //TODO: Check if any is there
        if (hexMapGenerator == null) { 
            hexMapGenerator = FindObjectOfType<HexMapGenerator>();
            
        }
        hexMapGenerator.GenerateMap();

        //Load UI


        // ....

    }
}
