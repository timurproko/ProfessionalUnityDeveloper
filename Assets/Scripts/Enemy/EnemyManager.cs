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
        [SerializeField] private Transform _worldTransform;
        [SerializeField] private Transform _container;
        [SerializeField] private Enemy _prefab;
        [SerializeField] private LevelsConfig _levelsConfig;
        [SerializeField] private int _poolPrewarmCount = 5;

        private ObjectPool<Enemy> _enemyPool;
        [SerializeField, ReadOnly] private int _totalSpawned;

        private void Awake()
        {
            _enemyPool = new ObjectPool<Enemy>(
                _prefab,
                _container,
                _worldTransform,
                _poolPrewarmCount
            );
        }

        private IEnumerator Start()
        {
            if (_levelsConfig == null || _levelsConfig.LevelCount == 0)
                yield break;

            for (int levelIndex = 0; levelIndex < _levelsConfig.LevelCount; levelIndex++)
            {
                LevelConfig level = _levelsConfig.GetLevel(levelIndex);
                if (level == null)
                    continue;

                int maxPerWave = level.MaxEnemiesPerWave;
                int totalToSpawnThisLevel = level.TotalEnemiesToSpawn;
                int totalSpawnedThisLevel = 0;

                while (totalSpawnedThisLevel < totalToSpawnThisLevel)
                {
                    yield return new WaitForSeconds(Random.Range(1, 2));

                    if (_enemyPool.ActiveCount >= maxPerWave)
                        continue;

                    Enemy enemy = _enemyPool.Get();

                    Transform spawnPosition = RandomPoint(spawnPositions);
                    enemy.transform.position = spawnPosition.position;

                    Transform attackPosition = RandomPoint(attackPositions);
                    enemy.SetDestination(attackPosition.position);
                    enemy.SetTarget(_target);

                    totalSpawnedThisLevel++;
                    _totalSpawned++;
                }
            }
        }

        private void Update()
        {
            foreach (Enemy enemy in _enemyPool.GetActiveSnapshot())
            {
                if (enemy.HealthComponent.Health <= 0)
                {
                    _enemyPool.Return(enemy);
                }
            }
        }

        private Transform RandomPoint(Transform[] points)
        {
            int index = Random.Range(0, points.Length);
            return points[index];
        }
    }
}