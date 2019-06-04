using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour, IConvertGameObjectToEntity
{
    [Header("Movement")]
    public float speed = 50f;

    [Header("Life Settings")]
    public float lifeTime = 1f;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        //life타임이 되면 해당 함수를 실행
        Invoke("RemoveProjectile", lifeTime);
    }

    private void Update()
    {
        Vector3 movement = transform.forward * speed * Time.deltaTime;
        rb.MovePosition(transform.position + movement);
    }

    void OnTriggerEnter(Collider theCollider)
    {
        if (theCollider.CompareTag("Enemy") || theCollider.CompareTag("Environment"))
            RemoveProjectile();
    }

    void RemoveProjectile()
    {
        Destroy(gameObject);
    }

    //Projectile을 Entity 청크로 변환
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent(entity, typeof(MoveForwardComponent));

        MoveSpeedComponent moveSpeed = new MoveSpeedComponent { Value = speed };
        dstManager.AddComponentData(entity, moveSpeed);

        TimeToLiveComponent timeToLive = new TimeToLiveComponent { Value = lifeTime };
        dstManager.AddComponentData(entity, timeToLive);
    }
}
