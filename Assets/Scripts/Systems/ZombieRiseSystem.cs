using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

[BurstCompile]
[UpdateBefore(typeof(TransformSystemGroup))]
[UpdateAfter(typeof(SpawnZombieSystem))]
public partial struct ZombieRiseSystem : ISystem
{

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        var commandBufferSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();

        new ZombieRiseJob
        {
            DeltaTime = deltaTime,
            CommandBuffer = commandBufferSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
        }.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct ZombieRiseJob : IJobEntity
{
    public float DeltaTime;
    public EntityCommandBuffer.ParallelWriter CommandBuffer;

    [BurstCompile]
    private void Execute(ZombieRiseAspect zombie, [ChunkIndexInQuery]int sortKey)
    {
        zombie.Rise(DeltaTime);

        if (!zombie.IsAboveGround)
            return;

        zombie.SetAtGroundLevel();
        CommandBuffer.RemoveComponent<ZombieRiseRate>(sortKey, zombie.Entity);
        CommandBuffer.SetComponentEnabled<ZombieWalkProperties>(sortKey, zombie.Entity, true);
    }
}