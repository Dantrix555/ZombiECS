using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[UpdateAfter(typeof(ZombieWalkSystem))]
public partial struct ZombieEatSystem : ISystem
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
        var brainRadius = brainScale * 5f + 1f;

        new ZombieEatJob
        {
            DeltaTime = deltaTime,
            CommandBuffer = commandBufferSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter(),
            BrainEntity = brainEntity,
            BrainRadiusSq = brainRadius * brainRadius
        }.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct ZombieEatJob : IJobEntity
{
    public float DeltaTime;
    public EntityCommandBuffer.ParallelWriter CommandBuffer;
    public Entity BrainEntity;
    public float BrainRadiusSq;

    [BurstCompile]
    private void Execute(ZombieEatAspect zombie, [EntityIndexInChunk] int sortKey)
    {
        if(zombie.IsInEatingRange(float3.zero, BrainRadiusSq))
        {
            zombie.Eat(DeltaTime, CommandBuffer, sortKey, BrainEntity);
        }
        else
        {
            CommandBuffer.SetComponentEnabled<ZombieEatProperties>(sortKey, zombie.Entity, false);
            CommandBuffer.SetComponentEnabled<ZombieWalkProperties>(sortKey, zombie.Entity, true);
        }
    }
}