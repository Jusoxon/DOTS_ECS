using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System;

[Serializable]
public struct MoveSpeedComponent : IComponentData
{
    public float Value;
}
