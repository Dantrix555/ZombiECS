using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class GraveyardMono : MonoBehaviour
{
    public float2 FieldsDimensions;
    public int NumberTombStonesToSpawn;
    public GameObject TombStonePrefab;
    public uint RandomSeed;
    public GameObject ZombiePrefab;
    public float ZombieSpawnRate;
}

public class GraveyardBaker : Baker<GraveyardMono>
{
    public override void Bake(GraveyardMono authoring)
    {
        var graveyardEntity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent(graveyardEntity, new GraveyardProperties
        {
            FieldsDimensions = authoring.FieldsDimensions,
            NumberTombStonesToSpawn = authoring.NumberTombStonesToSpawn,
            TombStonePrefab = GetEntity(authoring.TombStonePrefab),
            ZombiePrefab = GetEntity(authoring.ZombiePrefab),
            ZombieSpawnRate = authoring.ZombieSpawnRate
        });

        AddComponent(graveyardEntity, new GraveyardRandom
        {
            Value = Unity.Mathematics.Random.CreateFromIndex(authoring.RandomSeed)
        });

        AddComponent<ZombieSpawnPoints>(graveyardEntity);
        AddComponent<ZombieSpawnTimer>(graveyardEntity);
    }
}
