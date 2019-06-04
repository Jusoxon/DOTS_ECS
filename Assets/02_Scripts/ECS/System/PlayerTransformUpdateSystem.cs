using Unity.Entities;
using Unity.Transforms;

[UpdateBefore(typeof(CollisionSystem))]
public class PlayerTransformUpdateSystem : ComponentSystem
{
	protected override void OnUpdate()
	{
		if (GameMng.IsPlayerDead())
			return;

		Entities.WithAll<PlayerTagComponent>().ForEach((ref Translation pos) =>
		{
			pos = new Translation { Value = GameMng.PlayerPosition };
		});
	}
}