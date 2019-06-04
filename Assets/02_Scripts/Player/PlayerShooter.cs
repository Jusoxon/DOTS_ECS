using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [Header("Switch")]
    public bool spreadShot = false;
    public bool useECS = false;

    [Header("General")]
    public Transform spawnBulletPoint;
    public float fireRate = .5f;
    public int spreadAmount = 20;

    [Header("Bullet")]
    public GameObject bulletPrefab;

    float timer;

    EntityManager manager;
    Entity bulletEntityPrefabECS;

    //ECS 실행여부 확인
    private void Start()
    {
        if(useECS)
        {
            manager = World.Active.EntityManager;
            bulletEntityPrefabECS = GameObjectConversionUtility.ConvertGameObjectHierarchy(bulletPrefab, World.Active);
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        //쿨타임이 다 돌았을 때,
        if (Input.GetButton("Fire1") && timer >= fireRate)
        {
            Vector3 rotation = spawnBulletPoint.rotation.eulerAngles;
            rotation.x = 0f;

            if (useECS)
            {
                if (spreadShot)
                    SpawnBulletSpreadECS(rotation);
                else
                    SpawnBulletECS(rotation);
            }
            else
            {
                if (spreadShot)
                    SpawnBulletSpread(rotation);
                else
                    SpawnBullet(rotation);
            }

            timer = 0f;
        }

    }

    //일반적인 총알생성
    void SpawnBullet(Vector3 rotation)
    {
        GameObject bullet = Instantiate(bulletPrefab) as GameObject;

        bullet.transform.position = spawnBulletPoint.position;
        bullet.transform.rotation = Quaternion.Euler(rotation);
    }

    //방사형 총알생성
    void SpawnBulletSpread(Vector3 rotation)
    {
        //-x ~ x 의 너비
        int max = spreadAmount / 2;
        int min = -max;

        Vector3 tempRot = rotation;
        for (int x = min; x < max; x++)
        {
            //구형태의 방사
            tempRot.x = (rotation.x + 3 * x) % 360;

            for (int y = min; y < max; y++)
            {
                tempRot.y = (rotation.y + 3 * y) % 360;

                GameObject bullet = Instantiate(bulletPrefab) as GameObject;

                bullet.transform.position = spawnBulletPoint.position;
                bullet.transform.rotation = Quaternion.Euler(tempRot);
            }
        }
    }

    //ECS 총알생성
    void SpawnBulletECS(Vector3 rotation)
    {
        //ECS
        Entity bullet = manager.Instantiate(bulletEntityPrefabECS);

        manager.SetComponentData(bullet, new Translation { Value = spawnBulletPoint.position });
        manager.SetComponentData(bullet, new Rotation { Value = Quaternion.Euler(rotation) });
    }

    //ECS 방사형 총알생성
    void SpawnBulletSpreadECS(Vector3 rotation)
    {
        int max = spreadAmount / 2;
        int min = -max;
        int totalAmount = spreadAmount * spreadAmount;

        Vector3 tempRot = rotation;
        int index = 0;

        //ECS
        NativeArray<Entity> bullets = new NativeArray<Entity>(totalAmount, Allocator.TempJob);
        manager.Instantiate(bulletEntityPrefabECS, bullets);

        //산탄 범위
        for (int x = min; x < max; x++)
        {
            tempRot.x = (rotation.x + 3 * x) % 360;
        
            for (int y = min; y < max; y++)
            {
                tempRot.y = (rotation.y + 3 * y) % 360;
        
                manager.SetComponentData(bullets[index], new Translation { Value = spawnBulletPoint.position });
                manager.SetComponentData(bullets[index], new Rotation { Value = Quaternion.Euler(tempRot) });
        
                index++;
            }
        }
        bullets.Dispose();
    }
}