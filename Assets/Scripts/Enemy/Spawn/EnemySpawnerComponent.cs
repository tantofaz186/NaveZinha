using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct EnemySpawnerComponent : IComponentData
{
    public Entity enemyPrefab;
    public int numberOfEnemiesToSpawn;
    public int spawnIncrement;
    public int maxEnemiesToSpawn;
    public float spawnRadius;
    public float minimumSpawnDistance;

    public float timeUntilNextSpawn;
    public float spawnTimer;
}
