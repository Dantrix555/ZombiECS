using Unity.Burst;
using Unity.Entities;

[BurstCompile]
public partial struct SpawnZombieSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ZombieSpawnTimer>();
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var DeltaTime = SystemAPI.Time.DeltaTime;
        var CommandBuffer = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>();

        new SpawnZombieJob
        {
            DeltaTime = DeltaTime,
            CommandBuffer = CommandBuffer.CreateCommandBuffer(state.WorldUnmanaged)
        }.Schedule();
    }
}

[BurstCompile]
public partial struct SpawnZombieJob : IJobEntity
{
    public float DeltaTime;
    public EntityCommandBuffer CommandBuffer;

    [BurstCompile]
    private void Execute(GraveyardAspect graveyard)
    {
        graveyard.ZombieSpawnTimer -= DeltaTime;

        if (!graveyard.TimeToSpawnZombie || !graveyard.ZombieSpawnPointInitialized())
            return;

        graveyard.ZombieSpawnTimer = graveyard.ZombieSpawnRate;

        var zombieSpawn = CommandBuffer.Instantiate(graveyard.ZombiePrefab);
        var zombieSpawnTransform = graveyard.GetZombieSpawnPoint();

        CommandBuffer.SetComponent(zombieSpawn, zombieSpawnTransform);

        var zombieHeading = MathHelpers.GetHeading(zombieSpawnTransform.Position, graveyard.Position);
        CommandBuffer.SetComponent(zombieSpawn, new ZombieHeading { Value = zombieHeading });
    }
}