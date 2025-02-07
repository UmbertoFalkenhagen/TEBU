using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewScriptableResource", menuName = "TEBU/Map/ScriptableResource")]
public class ScriptableResource : ScriptableObject
{
    [Tooltip("The resource type associated with this Scriptable Resource (e.g., Rice, Herbs, etc.). If the resource you're looking for, you might have to add it to the Resources enum.")]
    public ResourceType resourcename;

    [Tooltip("The prefab representing this resource in the game world.")]
    public GameObject resourcePrefab;
}
