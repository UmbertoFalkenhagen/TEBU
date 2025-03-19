using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectType
{
    Tile,       // Will correspond to prefix '$'
    CityCenter, // Will correspond to prefix '#'
    Building,   // Will correspond to prefix '+'
    Animal      // Will correspond to prefix '~'
}

public enum TileType
{
    Grassland,
    Forest,
    Desert,
    Jungle,
    Hills,
    Mountains,
    Water
}

public enum ResourceType
{
    None,
    Wood,
    Rice,
    Herbs
}

public enum ProductType
{
    Bricks,
    Logs,
    Wheat,
    Rice,
    Roots,
    Tea,
    RiceMilk,
    CookedMeals
}

public enum AnimalType
{
    Cow,
    Sheep,
    Beaver,
    Squirrel
}

public enum BuildingType
{
    Brickhut,
    WheatFarm,
    RiceFarm,
    Mill,
    Woodcutter,
    RootHarvester,
    HerbHavester,
    Kitchen
}