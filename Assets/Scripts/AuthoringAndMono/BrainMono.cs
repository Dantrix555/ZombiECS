using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class BrainMono : MonoBehaviour
{
    public float BrainHealth;
}

public class BrainBaker : Baker<BrainMono>
{
    public override void Bake(BrainMono authoring)
    {
        var brainEntity = GetEntity(TransformUsageFlags.Dynamic);

        AddComponent<BrainTag>(brainEntity);
        AddComponent(brainEntity, new BrainHealth
        {
            Max = authoring.BrainHealth,
            Value = authoring.BrainHealth
        });

        AddBuffer<BrainDamageBufferElement>(brainEntity);
    }
}