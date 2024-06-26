using Enemy;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

[BurstCompile]
public partial class BulletSystem : SystemBase
{
    [BurstCompile]
    protected override void OnUpdate()
    {
        NativeArray<Entity> allEntities = EntityManager.GetAllEntities();

        PhysicsWorldSingleton physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>();

        foreach (Entity entity in allEntities)
        {
            if (EntityManager.HasComponent<BulletComponent>(entity)
                && EntityManager.HasComponent<BulletLifeTimeComponent>(entity))
            {
                LocalTransform bulletTransform = EntityManager.GetComponentData<LocalTransform>(entity);
                BulletComponent bulletComponent = EntityManager.GetComponentData<BulletComponent>(entity);

                bulletTransform.Position += bulletComponent.Speed * SystemAPI.Time.DeltaTime * bulletTransform.Forward() * -1;
                EntityManager.SetComponentData(entity, bulletTransform);

                BulletLifeTimeComponent bulletLifeTimeComponent = EntityManager.GetComponentData<BulletLifeTimeComponent>(entity);
                bulletLifeTimeComponent.RemainingLifeTime -= SystemAPI.Time.DeltaTime;

                if (bulletLifeTimeComponent.RemainingLifeTime <= 0)
                {
                    EntityManager.DestroyEntity(entity);
                    continue;
                }

                EntityManager.SetComponentData(entity, bulletLifeTimeComponent);

                NativeList<ColliderCastHit> colliderCastHits = new NativeList<ColliderCastHit>(Allocator.Temp);

                float3 pointA = bulletTransform.Position - bulletTransform.Forward() * 0.15f;
                float3 pointB = bulletTransform.Position + bulletTransform.Forward() * 0.15f;

                physicsWorldSingleton.CapsuleCastAll(pointA, pointB, bulletComponent.Size / 2, bulletTransform.Forward(), 1f,
                    ref colliderCastHits
                    , new CollisionFilter
                    {
                        BelongsTo = (uint)CollisionLayer.Default,
                        CollidesWith = ((uint)CollisionLayer.Wall | (uint)CollisionLayer.Enemy),
                    }
                );

                if (colliderCastHits.Length > 0)
                {
                    for (int i = 0; i < colliderCastHits.Length; i++)
                    {
                        Debug.Log(colliderCastHits[i].Entity);
                        Entity hitEntity = colliderCastHits[i].Entity;
                        if (EntityManager.HasComponent<EnemyComponent>(hitEntity))
                        {
                            EnemyComponent enemyComponent = EntityManager.GetComponentData<EnemyComponent>(hitEntity);
                            enemyComponent.Health -= bulletComponent.Damage;
                            EntityManager.SetComponentData(hitEntity, enemyComponent);

                            if (enemyComponent.Health <= 0)
                            {
                                EntityManager.DestroyEntity(hitEntity);
                            }
                            EntityManager.DestroyEntity(entity);
                        }
                    }
                }

                colliderCastHits.Dispose();
            }
        }
    }
}