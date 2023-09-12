using Unity.Entities;
using Unity.Mathematics;

public struct ZombieSpawnPoints : IComponentData
{
    public BlobAssetReference<ZombieSpawnPointsBlob> Value;
}

public struct ZombieSpawnPointsBlob
{
    public BlobArray<float3> Value;
}