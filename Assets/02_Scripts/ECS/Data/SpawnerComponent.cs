using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Entities;
using UnityEngine;

[Serializable]
public class SpawnerComponent : ISharedComponentData
{
    public float coolTime;
    public GameObject prefab;
}
