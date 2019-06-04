using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class PlayerToEntityConversion : MonoBehaviour, IConvertGameObjectToEntity
{
    public float healthValue = 5f;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent(entity, typeof(PlayerTagComponent));

        HealthComponent health = new HealthComponent { Value = healthValue };
        dstManager.AddComponentData(entity, health);
    }
}
