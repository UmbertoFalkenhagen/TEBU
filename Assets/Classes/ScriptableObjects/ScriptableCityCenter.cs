using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewScriptableCityCenter", menuName = "TEBU/Content/ScriptableCityCenter")]
public class ScriptableCityCenter : ScriptableObject
{
    [Tooltip("The tile type where this city center can be constructed (e.g., Grassland, Plains).")]
    public TileType cityLocation;

    [Tooltip("The prefab representing the initial version of the city center.")]
    public GameObject basicPrefab;

    [Tooltip("The prefab representing the upgraded version of the city center.")]
    public GameObject upgradedPrefab;
}
