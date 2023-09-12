using Unity.Entities;

public struct ZombieEatProperties : IComponentData, IEnableableComponent
{
    public float EatDamagePerSecond;
    public float EatAmplitude;
    public float EatFrequency;
}
