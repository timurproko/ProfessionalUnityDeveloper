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
        [SerializeField] private LevelConfig levelConfig;
        [SerializeField] private BulletConfig bulletConfig;

        private ObjectPool<Enemy> enemyPool;
        [SerializeField, ReadOnly] private int totalSpawned;

        private void Awake()
        {
            int prewarm = levelConfig.PoolPrewarmCount;
            
            enemyPool = new ObjectPool<Enemy>(
                prefab,
                container,
                worldTransform,
                prewarm
            );
        }

        private IEnumerator Start()
        {
            int maxPerWave = levelConfig.MaxEnemiesPerWave;
            int totalToSpawn = levelConfig.TotalEnemiesToSpawn;

            while (totalSpawned < totalToSpawn)
            {
                yield return new WaitForSeconds(Random.Range(1, 2));

                if (enemyPool.ActiveCount >= maxPerWave)
                    continue;

                Enemy enemy = enemyPool.Get();
                totalSpawned++;

                Transform spawnPosition = RandomPoint(spawnPositions);
                enemy.transform.position = spawnPosition.position;

                Transform attackPosition = RandomPoint(attackPositions);
                enemy.SetDestination(attackPosition.position);
                enemy.Target = _target;

                enemy.OnFire += OnFire;
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

        private void OnFire(Vector2 position, Vector2 direction)
        {
            BulletSpawnRequest request = bulletConfig.CreateRequest(position, direction);
            bulletSystem.SpawnBullet(request);
        }

        private Transform RandomPoint(Transform[] points)
        {
            int index = Random.Range(0, points.Length);
            return points[index];
        }
    }
}