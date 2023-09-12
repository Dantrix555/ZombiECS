using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct SpawnTombstoneSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GraveyardProperties>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        state.Enabled = false;

        var graveyardEntity = SystemAPI.GetSingletonEntity<GraveyardProperties>();
        var graveyard = SystemAPI.GetAspect<GraveyardAspect>(graveyardEntity);

        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        var builder = new BlobBuilder(Allocator.Temp);
        ref var spawnPoints = ref builder.ConstructRoot<ZombieSpawnPointsBlob>();
        var arrayBuilder = builder.Allocate(ref spawnPoints.Value, graveyard.NumberTombstonesToSpawn);

        var tombstoneOffset = new float3(0f, -2f, 1f);

        for(var i = 0; i < graveyard.NumberTombstonesToSpawn; i++)
        {
            var tombstoneSpawn = commandBuffer.Instantiate(graveyard.TombstonePrefab);
            var tombstoneSpawnTransform = graveyard.GetRandomTombstoneTransform();
            commandBuffer.SetComponent(tombstoneSpawn, tombstoneSpawnTransform);

            var newZombieSpawnPoint = tombstoneSpawnTransform.Position + tombstoneOffset;
            arrayBuilder[i] = newZombieSpawnPoint;
        }

        var blobAsset = builder.CreateBlobAssetReference<ZombieSpawnPointsBlob>(Allocator.Persistent);
        commandBuffer.SetComponent(graveyardEntity, new ZombieSpawnPoints { Value = blobAsset });
        builder.Dispose();

        commandBuffer.Playback(state.EntityManager);
    }
}
