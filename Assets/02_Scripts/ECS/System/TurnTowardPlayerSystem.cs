using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(MoveForwardSystem))]
public class TurnTowardPlayerSystem : JobComponentSystem
{
    [BurstCompile]
    [RequireComponentTag(typeof(EnemyTagComponent))]    //Enemy만 적용하기 위해 필요
    struct TurnJob : IJobForEach<Translation, Rotation>
    {
        public float3 playerPosition;

        public void Execute([ReadOnly] ref Translation pos, ref Rotation rot)
        {
            float3 heading = playerPosition - pos.Value;
            heading.y = 0f;
            rot.Value = quaternion.LookRotation(heading, math.up());
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        if (GameMng.IsPlayerDead())
            return inputDeps;

        var job = new TurnJob
        {
            playerPosition = GameMng.PlayerPosition
        };

        return job.Schedule(this, inputDeps);
    }


}
