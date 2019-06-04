using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Switch")]
    public bool spawnEnemies = false;
    public bool useECS = false;

    [Header("Enemy Spawn Info")]
    public float enemySpawnRadius = 10f;
    public GameObject enemyPrefab;

    [Header("Enemy Spawn Timing")]
    [Range(1, 100)] public int spawnsPerInterval = 1;
    [Range(.1f, 2f)] public float spawnInterval = 1f;

    EntityManager manager;
    Entity enemyEntityPrefab;

    float coolTime;

    private void Start()
    {
        if(useECS)
        {
            manager = World.Active.EntityManager;
            enemyEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(enemyPrefab, World.Active);
        }
    }

    void Update()
    {
        if (!spawnEnemies || GameMng.IsPlayerDead())
            return;

        coolTime -= Time.deltaTime;

        if (coolTime <= 0f)
        {
            coolTime += spawnInterval;
            Spawn();
        }
    }

    void Spawn()
    {
        for (int i = 0; i < spawnsPerInterval; i++)
        {
            Vector3 pos = GameMng.GetPositionAroundPlayer(enemySpawnRadius);

            if (!useECS)
            {
                Instantiate(enemyPrefab, pos, Quaternion.identity);
            }
            //ECS에 Entity 청크로 변환된 Enemy를 소환
            else
            {
                Entity enemy = manager.Instantiate(enemyEntityPrefab);
                manager.SetComponentData(enemy, new Translation { Value = pos });
            }
        }
    }
}
