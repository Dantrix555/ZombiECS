using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public readonly partial struct GraveyardAspect : IAspect
{
    #region Fields and properties

    public readonly Entity Entity;

    private readonly RefRO<LocalTransform> _transform;
    private LocalTransform Transform => _transform.ValueRO;

    private readonly RefRO<GraveyardProperties> _graveyardProperties;
    private readonly RefRW<GraveyardRandom> _graveyardRandom;
    private readonly RefRW<ZombieSpawnPoints> _zombieSpawnPoints;
    private readonly RefRW<ZombieSpawnTimer> _zombieSpawnTimer;

    public int NumberTombstonesToSpawn => _graveyardProperties.ValueRO.NumberTombStonesToSpawn;
    public Entity TombstonePrefab => _graveyardProperties.ValueRO.TombStonePrefab;

    public bool ZombieSpawnPointInitialized()
    {
        return _zombieSpawnPoints.ValueRO.Value.IsCreated && ZombieSpawnPointCount > 0;
    }

    private int ZombieSpawnPointCount => _zombieSpawnPoints.ValueRO.Value.Value.Value.Length;

    private const float BRAIN_SAFETY_RADIUS_SQ = 100;

    private float3 MinCorner => _transform.ValueRO.Position - HalfDimensions;
    private float3 MaxCorner => _transform.ValueRO.Position + HalfDimensions;
    
    private float3 HalfDimensions => new()
    {
        x = _graveyardProperties.ValueRO.FieldsDimensions.x * 0.5f,
        y = 0,
        z = _graveyardProperties.ValueRO.FieldsDimensions.y * 0.5f
    };

    public float ZombieSpawnTimer
    {
        get => _zombieSpawnTimer.ValueRO.Value;
        set => _zombieSpawnTimer.ValueRW.Value = value;
    }

    public bool TimeToSpawnZombie => ZombieSpawnTimer <= 0;

    public float ZombieSpawnRate => _graveyardProperties.ValueRO.ZombieSpawnRate;

    public Entity ZombiePrefab => _graveyardProperties.ValueRO.ZombiePrefab;

    public float3 Position => _transform.ValueRO.Position;

    #endregion

    #region Public Methods

    public LocalTransform GetRandomTombstoneTransform()
    {
        return new LocalTransform
        {
            Position = GetRandomPosition(),
            Rotation = GetRandomRotation(),
            Scale = GetRandomScale(0.5f)
        };
    }

    public float2 GetRandomOffset()
    {
        return _graveyardRandom.ValueRW.Value.NextFloat2();
    }

    public LocalTransform GetZombieSpawnPoint()
    {
        var position = GetRandomPosition();

        return new LocalTransform
        {
            Position = position,
            Rotation = quaternion.RotateY(MathHelpers.GetHeading(position, _transform.ValueRO.Position)),
            Scale = 1f
        };
    }

    #endregion

    #region Inner Methods

    private float3 GetRandomPosition()
    {
        float3 randomPosition;

        do
        {
            randomPosition = _graveyardRandom.ValueRW.Value.NextFloat3(MinCorner, MaxCorner);

        } while (math.distancesq(_transform.ValueRO.Position, randomPosition) <= BRAIN_SAFETY_RADIUS_SQ);


        return randomPosition;
    }

    private quaternion GetRandomRotation() => quaternion.RotateY(_graveyardRandom.ValueRW.Value.NextFloat(-0.25f, 0.25f));
    private float GetRandomScale(float minScale) => _graveyardRandom.ValueRW.Value.NextFloat(minScale, 1f);

    private float3 GetRandomZombieSpawnPoint()
    {
        return GetZombieSpawnPoint(_graveyardRandom.ValueRW.Value.NextInt(ZombieSpawnPointCount));
    }

    private float3 GetZombieSpawnPoint(int i) => _zombieSpawnPoints.ValueRO.Value.Value.Value[i];

    #endregion
}
