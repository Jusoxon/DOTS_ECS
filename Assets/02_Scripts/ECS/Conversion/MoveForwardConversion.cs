using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class MoveForwardConversion : MonoBehaviour, IConvertGameObjectToEntity
{
    public float speed = 50f;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent(entity, typeof(MoveForwardComponent));

        MoveSpeedComponent moveSpeed = new MoveSpeedComponent { Value = speed };
        dstManager.AddComponentData(entity, moveSpeed);
    }
}
