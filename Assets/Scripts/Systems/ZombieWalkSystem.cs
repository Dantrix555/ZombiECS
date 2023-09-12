using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateAfter(typeof(ZombieRiseSystem))]
public partial struct ZombieWalkSystem : ISystem
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
        var brainEntity = SystemAPI.GetSingletonEntity<BrainTag>();
        var brainScale = SystemAPI.GetComponent<LocalTransform>(brainEntity).Scale;
        var brainRadius = brainScale * 5f + 0.5f;

        new ZombieWalkJob
        {
            DeltaTime = deltaTime,
            BrainRadiusSq = brainRadius * brainRadius,
            CommandBuffer = commandBufferSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
        }.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct ZombieWalkJob : IJobEntity
{
    public float DeltaTime;
    public float BrainRadiusSq;
    public EntityCommandBuffer.ParallelWriter CommandBuffer;

    [BurstCompile]
    private void Execute(ZombieWalkAspect zombie, [ChunkIndexInQuery] int sortKey)
    {
        zombie.Walk(DeltaTime);

        if(zombie.IsInStoppingRange(float3.zero, BrainRadiusSq))
        {
            CommandBuffer.SetComponentEnabled<ZombieWalkProperties>(sortKey, zombie.Entity, false);
            CommandBuffer.SetComponentEnabled<ZombieEatProperties>(sortKey, zombie.Entity, true);
        }
    }
}