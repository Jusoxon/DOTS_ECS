using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour, IConvertGameObjectToEntity
{
    [Header("Movement")]
    public float speed = 2f;

    [Header("Life Setting")]
    public float healthEnemy = 1f;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        //플레이어가 살아있다면
        if(!GameMng.IsPlayerDead())
        {
            Vector3 desire = GameMng.PlayerPosition - transform.position;
            desire.y = 0;
            transform.rotation = Quaternion.LookRotation(desire);
        }
        //플레이어가 죽었으면
        else
        {
            transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Bullet")) return;

        if(--healthEnemy <= 0)
        {
            Destroy(gameObject);
        }
    }

    //Enemy를 Entity 청크로 변환
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent(entity, typeof(EnemyTagComponent));
        dstManager.AddComponent(entity, typeof(MoveForwardComponent));

        MoveSpeedComponent moveSpeed = new MoveSpeedComponent { Value = speed };
        dstManager.AddComponentData(entity, moveSpeed);

        HealthComponent health = new HealthComponent { Value = healthEnemy };
        dstManager.AddComponentData(entity, health);
    }
}
