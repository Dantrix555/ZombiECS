using Unity.Entities;
using Unity.Mathematics;

public struct GraveyardProperties : IComponentData
{
    public float2 FieldsDimensions;
    public int NumberTombStonesToSpawn;
    public Entity TombStonePrefab;
    public Entity ZombiePrefab;
    public float ZombieSpawnRate;
}

public struct ZombieSpawnTimer : IComponentData
{
    public float Value;
}