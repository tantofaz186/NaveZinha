using Enemy;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class EnemySpawnerSystem : SystemBase
{
    private Entity enemySpawnerEntity;
    private EnemySpawnerComponent enemySpawnerComponent;

    private Entity playerEntity;

    private Unity.Mathematics.Random random;

    protected override void OnCreate()
    {
        base.OnCreate();
        random = Unity.Mathematics.Random.CreateFromIndex((uint)enemySpawnerComponent.GetHashCode());
    }

    protected override void OnUpdate()
    {
        enemySpawnerEntity = SystemAPI.GetSingletonEntity<EnemySpawnerComponent>();
        enemySpawnerComponent = EntityManager.GetComponentData<EnemySpawnerComponent>(enemySpawnerEntity);

        playerEntity = SystemAPI.GetSingletonEntity<PlayerComponent>();
        PlayerComponent playerComponent = EntityManager.GetComponentData<PlayerComponent>(playerEntity);

        SpawnEnemies(ref CheckedStateRef);
    }

    private void SpawnEnemies(ref SystemState state)
    {
        enemySpawnerComponent.spawnTimer -= SystemAPI.Time.DeltaTime;
        if (enemySpawnerComponent.spawnTimer <= 0)
        {
            for (int i = 0; i < enemySpawnerComponent.numberOfEnemiesToSpawn; i++)
            {
                EntityCommandBuffer ECB = new EntityCommandBuffer(Allocator.Temp);
                Entity enemyEntity = EntityManager.Instantiate(enemySpawnerComponent.enemyPrefab);

                LocalTransform enemyTransform = EntityManager.GetComponentData<LocalTransform>(enemyEntity);
                LocalTransform playerTransform = EntityManager.GetComponentData<LocalTransform>(playerEntity);

                float minDistanceSqared = enemySpawnerComponent.minimumSpawnDistance * enemySpawnerComponent.minimumSpawnDistance;
                float2 randomOffset = random.NextFloat2Direction() *
                                      random.NextFloat(enemySpawnerComponent.minimumSpawnDistance, enemySpawnerComponent.spawnRadius);
                float2 playerPosition = new float2(playerTransform.Position.x, playerTransform.Position.z);
                float2 spawnPosition = playerPosition + randomOffset;
                float distanceSquared = math.lengthsq(spawnPosition - playerPosition);

                if (distanceSquared < minDistanceSqared)
                {
                    spawnPosition = playerPosition + math.normalize(randomOffset) * math.sqrt(minDistanceSqared);
                }

                enemyTransform.Position = new float3(spawnPosition.x, 0, spawnPosition.y);

                float3 direction = math.normalize(playerTransform.Position - enemyTransform.Position);
                float angle = math.degrees(math.atan2(direction.z, direction.x));
                angle -= 90;
                enemyTransform.Rotation = Quaternion.AngleAxis(angle, Vector3.up);

                ECB.SetComponent(enemyEntity, enemyTransform);

                ECB.AddComponent(enemyEntity, new EnemyComponent
                {
                    MoveSpeed = 2f,
                    Health = 100f,
                });


                ECB.Playback(EntityManager);
                ECB.Dispose();
            }

            enemySpawnerComponent.numberOfEnemiesToSpawn += enemySpawnerComponent.spawnIncrement;
            enemySpawnerComponent.numberOfEnemiesToSpawn =
                Mathf.Min(enemySpawnerComponent.numberOfEnemiesToSpawn, enemySpawnerComponent.maxEnemiesToSpawn);
            enemySpawnerComponent.spawnTimer = enemySpawnerComponent.timeUntilNextSpawn;
        }


        EntityManager.SetComponentData(enemySpawnerEntity, enemySpawnerComponent);
    }
}