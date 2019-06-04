using Unity.Entities;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class RemoveDeadSystem : ComponentSystem
{
	protected override void OnUpdate()
	{
		Entities.ForEach((Entity entity, ref HealthComponent health, ref Translation pos) =>
		{
			if (health.Value <= 0)
			{
				if (EntityManager.HasComponent(entity, typeof(PlayerTagComponent)))
				{
					GameMng.PlayerDied();
				}

				else if (EntityManager.HasComponent(entity, typeof(EnemyTagComponent)))
				{
					PostUpdateCommands.DestroyEntity(entity);
				}
			}
		});
	}
}