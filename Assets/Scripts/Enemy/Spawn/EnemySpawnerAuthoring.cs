using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EnemySpawnerAuthoring : MonoBehaviour
{
    public GameObject enemyPrefab;

    public int numberOfEnemiesToSpawn = 50;
    public int spawnIncrement = 2;
    public int maxEnemiesToSpawn = 200;
    public float spawnRadius = 40;
    public float minimumSpawnDistance = 5;

    public float timeUntilNextSpawn = 2;

    public class EnemySpawnerBaker : Baker<EnemySpawnerAuthoring>
    {
        public override void Bake(EnemySpawnerAuthoring authoring)
        {
            Entity enemySpawnerEntity = GetEntity(TransformUsageFlags.None);

            AddComponent(enemySpawnerEntity, new EnemySpawnerComponent
            {
                enemyPrefab = GetEntity(authoring.enemyPrefab, TransformUsageFlags.None),
                numberOfEnemiesToSpawn = authoring.numberOfEnemiesToSpawn,
                spawnIncrement = authoring.spawnIncrement,
                maxEnemiesToSpawn = authoring.maxEnemiesToSpawn,
                spawnRadius = authoring.spawnRadius,
                minimumSpawnDistance = authoring.minimumSpawnDistance,
                timeUntilNextSpawn = authoring.timeUntilNextSpawn,
                spawnTimer = 0
            });
        }
    }
}
