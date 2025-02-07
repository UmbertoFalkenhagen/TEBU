using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewScriptableAnimal", menuName = "TEBU/Content/ScriptableAnimal")]
public class ScriptableAnimal : ScriptableObject
{
    [Tooltip("The name of the animal (e.g., Cow, Sheep, Beaver, etc.). If the animal you're looking for, you might have to add it to the Animals enum.")]
    public AnimalType animalName;

    [Tooltip("The prefab representing this animal in the game world.")]
    public GameObject prefab;

    [Tooltip("The tile type where this animal can spawn (e.g., Grassland, Forest).")]
    public TileType spawnLocation;

    [Tooltip("The basic food product required for this animal to start spawning in a city.")]
    public ProductType basicFood;

    [Header("Basic Ability Unlocks")]
    [Tooltip("The product required to unlock the first basic ability of this animal.")]
    public ProductType basicAbilityUnlockProduct1;

    [Tooltip("The product required to unlock the second basic ability of this animal.")]
    public ProductType basicAbilityUnlockProduct2;

    [Header("Ability Improvements")]
    [Tooltip("The product required to improve the first ability.")]
    public ProductType abilityImprovementProduct1;

    [Tooltip("The product required to improve the second ability.")]
    public ProductType abilityImprovementProduct2;

    // Placeholder for future ability system
    // public ScriptableAbility[] abilities;
}
