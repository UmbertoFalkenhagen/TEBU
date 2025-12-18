using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewScriptableAnimal", menuName = "TEBU/Content/ScriptableAnimal")]
public class ScriptableAnimal : ScriptableObject
{
    [Tooltip("The name of the animal (e.g., Cow, Sheep, Beaver, etc.).")]
    public AnimalType animalName;

    [Tooltip("The prefab representing this animal in the game world.")]
    public GameObject prefab;

    [Tooltip("The tile types where this animal can spawn (e.g., Grassland, Forest).")]
    public List<TileType> requiredTileTypes;

    [Tooltip("The basic food product required for this animal to start spawning in a city.")]
    public ProductType basicFood;

    [Header("Basic Ability Unlocks")]
    [Tooltip("The product required to unlock the first basic ability of this animal.")]
    public ProductType ability1UnlockProduct;

    [Tooltip("The product required to unlock the second basic ability of this animal.")]
    public ProductType ability2UnlockProduct;

    [Header("Ability Improvements")]
    [Tooltip("The product required to improve the first ability.")]
    public ProductType ability1ImprovProduct;

    [Tooltip("The product required to improve the second ability.")]
    public ProductType ability2ImprovProduct;
}
