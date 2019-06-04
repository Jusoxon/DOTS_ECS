using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateAfter(typeof(MoveForwardSystem))]
[UpdateBefore(typeof(TimedDestroySystem))]
public class CollisionSystem : JobComponentSystem
{
	EntityQuery enemyGroup;
	EntityQuery bulletGroup;
	EntityQuery playerGroup;

	protected override void OnCreate()
	{
		playerGroup = GetEntityQuery(typeof(HealthComponent), ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<PlayerTagComponent>()); //player가 가지고 있어야 하는 컴포넌트들
		enemyGroup = GetEntityQuery(typeof(HealthComponent), ComponentType.ReadOnly<Translation>(), ComponentType.ReadOnly<EnemyTagComponent>());   //Enemy가 가지고 있어야 하는 컴포넌트들
		bulletGroup = GetEntityQuery(typeof(TimeToLiveComponent), ComponentType.ReadOnly<Translation>());                                           //Projectile이 가지고 있어야 하는 컴포넌트들
	}

	[BurstCompile]
	struct CollisionJob : IJobChunk
	{
		public float radius;

		public ArchetypeChunkComponentType<HealthComponent> healthType;
		[ReadOnly] public ArchetypeChunkComponentType<Translation> translationType;

		[DeallocateOnJobCompletion]
		[ReadOnly] public NativeArray<Translation> transToTestAgainst;

        //청크당 Hp와 Collision을 설정, Collision Enter시의 계산
		public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
		{
			var chunkHealths = chunk.GetNativeArray(healthType);
			var chunkTranslations = chunk.GetNativeArray(translationType);

			for (int i = 0; i < chunk.Count; i++)
			{
				float damage = 0f;
                HealthComponent health = chunkHealths[i];
				Translation pos = chunkTranslations[i];

				for (int j = 0; j < transToTestAgainst.Length; j++)
				{
					Translation pos2 = transToTestAgainst[j];

					if (CheckCollision(pos.Value, pos2.Value, radius))
					{
						damage += 1;
					}
				}

				if (damage > 0)
				{
					health.Value -= damage;
					chunkHealths[i] = health;
				}
			}
		}
	}

	protected override JobHandle OnUpdate(JobHandle inputDependencies)
	{
		var healthType = GetArchetypeChunkComponentType<HealthComponent>(false);
		var translationType = GetArchetypeChunkComponentType<Translation>(true);

		float enemyRadius = GameMng.EnemyCollisionRadius;
		float playerRadius = GameMng.PlayerCollisionRadius;

        //Enemy와 Bullet
		var jobEvB = new CollisionJob()
		{
			radius = enemyRadius * enemyRadius,
			healthType = healthType,
			translationType = translationType,
			transToTestAgainst = bulletGroup.ToComponentDataArray<Translation>(Allocator.TempJob)
		};

		JobHandle jobHandle = jobEvB.Schedule(enemyGroup, inputDependencies);

		if (GameMng.IsPlayerDead())
			return jobHandle;

		var jobPvE = new CollisionJob()
		{
			radius = playerRadius * playerRadius,
			healthType = healthType,
			translationType = translationType,
			transToTestAgainst = enemyGroup.ToComponentDataArray<Translation>(Allocator.TempJob)
		};

		return jobPvE.Schedule(playerGroup, jobHandle);
	}
    //Collision끼리 닿았는지 확인
	static bool CheckCollision(float3 posA, float3 posB, float radiusSqr)
	{
		float3 delta = posA - posB;
		float distanceSquare = delta.x * delta.x + delta.z * delta.z;

		return distanceSquare <= radiusSqr;
	}
}
