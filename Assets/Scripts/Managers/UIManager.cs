using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance { get; private set; }
    private DatabaseManager databaseManager;
    public BuildingPanel buildingPanel; //TODO: atm added via inspector


    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }

        Instance = this;
        DontDestroyOnLoad(gameObject);


    }
    // Start is called before the first frame update
    void Start()
    {
        databaseManager = DatabaseManager.Instance;
        if (databaseManager == null)
        {
            Debug.LogError("DatabaseManager could not be found!");
        }
        ActiveTile.OnActiveTileChanged += tileClick;

    }
    private void OnDestroy()
    {
        ActiveTile.OnActiveTileChanged -= tileClick;
    }

    public void tileClick(HexTile activeTile)
    {
        Debug.Log("Tile Active:" + ActiveTile.Instance.GetActiveTile().ToString());
        if (activeTile.heldBuilding != null)
        {
            ObjectIdentifier buildingID = activeTile.GetHeldBuildingID();
            ObjectIdentifier cityID = activeTile.GetHeldCityID();
            if (buildingID == null && cityID != null)
            {
                //city ist hier logic
                //1. update resourceBar based on
                //                   ResourceBar.Instance.UpdateResourceBar(tileID, databaseManager);

                //show claims
                // show tile Infomation in right menu
            }
            else if (buildingID != null && cityID == null)
            {
                //building ist hier logic


                //Update Building Panel

                //get parent city center and update resource bar based on it
                //  resourceScript.UpdateResourceBar(tileID, databaseManager);

                //show claims
                //show tile Information in right menu
            }
        }
        else
        {
            //logic wenn tile leer ist
            buildingPanel.UpdateBuildingOptions(activeTile);  //show build options
                                                              //show claims 
                                                              //show tile Informations in rechten menu
        }
    }


}
