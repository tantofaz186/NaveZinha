using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;

public enum CollisionLayer
{
    Default = 1 << 0, Wall = 6 << 0, Enemy = 7 << 0,
}