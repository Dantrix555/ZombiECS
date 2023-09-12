using Unity.Burst;
using Unity.Entities;
using Unity.Collections;

[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct InitializeZombieSystem : ISystem
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
        var commandBuffer = new EntityCommandBuffer(Allocator.Temp);

        foreach(var zombie in SystemAPI.Query<ZombieWalkAspect>().WithAll<NewZombieTag>())
        {
            commandBuffer.RemoveComponent<NewZombieTag>(zombie.Entity);
            commandBuffer.SetComponentEnabled<ZombieWalkProperties>(zombie.Entity, false);
            commandBuffer.SetComponentEnabled<ZombieEatProperties>(zombie.Entity, false);
        }

        commandBuffer.Playback(state.EntityManager);
    }
}
