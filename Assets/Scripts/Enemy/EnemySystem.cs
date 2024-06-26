using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Enemy
{
    public partial class EnemySystem : SystemBase
    {
        Entity playerEntity;

        protected override void OnUpdate()
        {
            playerEntity = SystemAPI.GetSingletonEntity<PlayerComponent>();
            LocalTransform playerTransform = EntityManager.GetComponentData<LocalTransform>(playerEntity);

            NativeArray<Entity> allEntities = EntityManager.GetAllEntities();
            foreach (Entity entity in allEntities)
            {
                if (EntityManager.HasComponent<EnemyComponent>(entity))
                {
                    LocalTransform enemyTransform = EntityManager.GetComponentData<LocalTransform>(entity);
                    EnemyComponent enemyComponent = EntityManager.GetComponentData<EnemyComponent>(entity);
                    float3 moveDirection = math.normalize(playerTransform.Position - enemyTransform.Position);
                    float3 move = moveDirection * enemyComponent.MoveSpeed * SystemAPI.Time.DeltaTime;
                    enemyTransform.Position += move;
                    float3 direction = math.normalize(playerTransform.Position - enemyTransform.Position);
                    float angle = math.degrees(math.atan2(direction.z, direction.x));
                    angle -= 90;
                    enemyTransform.Rotation = Quaternion.AngleAxis(angle, -Vector3.up);

                    EntityManager.SetComponentData(entity, enemyTransform);
                }
            }
        }
    }
}