using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ShootEmUp
{
    public sealed class EnemyManager : MonoBehaviour
    {
        [SerializeField] private Transform[] spawnPositions;
        [SerializeField] private Transform[] attackPositions;
        [SerializeField] private Player _target;
        [SerializeField] private Transform worldTransform;
        [SerializeField] private Transform container;
        [SerializeField] private Enemy prefab;
        [SerializeField] private BulletManager bulletSystem;
        [SerializeField] private LevelsConfig levelsConfig;
        [SerializeField] private int poolPrewarmCount = 5;

        private ObjectPool<Enemy> enemyPool;
        [SerializeField, ReadOnly] private int totalSpawned;

        private void Awake()
        {
            enemyPool = new ObjectPool<Enemy>(
                prefab,
                container,
                worldTransform,
                poolPrewarmCount
            );
        }

        private IEnumerator Start()
        {
            if (levelsConfig == null || levelsConfig.LevelCount == 0)
                yield break;

            for (int levelIndex = 0; levelIndex < levelsConfig.LevelCount; levelIndex++)
            {
                LevelConfig level = levelsConfig.GetLevel(levelIndex);
                if (level == null)
                    continue;

                int maxPerWave = level.MaxEnemiesPerWave;
                int totalToSpawnThisLevel = level.TotalEnemiesToSpawn;
                int totalSpawnedThisLevel = 0;

                while (totalSpawnedThisLevel < totalToSpawnThisLevel)
                {
                    yield return new WaitForSeconds(Random.Range(1, 2));

                    if (enemyPool.ActiveCount >= maxPerWave)
                        continue;

                    Enemy enemy = enemyPool.Get();

                    Transform spawnPosition = RandomPoint(spawnPositions);
                    enemy.transform.position = spawnPosition.position;

                    Transform attackPosition = RandomPoint(attackPositions);
                    enemy.SetDestination(attackPosition.position);
                    enemy.Target = _target;

                    enemy.OnFire += OnFire;

                    totalSpawnedThisLevel++;
                    totalSpawned++;
                }
            }
        }

        private void FixedUpdate()
        {
            foreach (Enemy enemy in enemyPool.GetActiveSnapshot())
            {
                if (enemy.HealthComponent.Health <= 0)
                {
                    enemy.OnFire -= OnFire;
                    enemyPool.Return(enemy);
                }
            }
        }

        private void OnFire(BulletSpawnRequest request)
        {
            bulletSystem.SpawnBullet(request);
        }

        private Transform RandomPoint(Transform[] points)
        {
            int index = Random.Range(0, points.Length);
            return points[index];
        }
    }
}