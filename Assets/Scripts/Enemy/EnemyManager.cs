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

        private const int PoolPrewarmCount = 5;
        private const float SpawnDelayMin = 1f;
        private const float SpawnDelayMax = 2f;

        private ObjectPool<Enemy> _enemyPool;
        private int _currentLevelIndex;
        private int _totalSpawnedThisLevel;
        private float _nextSpawnTime;
        
        [SerializeField, ReadOnly] private int _totalSpawned;

        private void Awake()
        {
            _enemyPool = new ObjectPool<Enemy>(
                _prefab,
                _container,
                _worldTransform,
                PoolPrewarmCount
            );
        }

        private void Update()
        {
            ReturnDeadEnemies();
            if (TrySpawnEnemy())
                _nextSpawnTime = Time.time + Random.Range(SpawnDelayMin, SpawnDelayMax);
        }

        private void ReturnDeadEnemies()
        {
            foreach (Enemy enemy in _enemyPool.GetActiveSnapshot())
            {
                if (!enemy.IsAlive)
                    _enemyPool.Return(enemy);
            }
        }

        private bool TrySpawnEnemy()
        {
            if (_levelsConfig == null || _levelsConfig.LevelCount == 0)
                return false;
            if (_currentLevelIndex >= _levelsConfig.LevelCount)
                return false;
            if (Time.time < _nextSpawnTime)
                return false;

            LevelConfig level = _levelsConfig.GetLevel(_currentLevelIndex);
            if (level == null)
                return false;

            if (_totalSpawnedThisLevel >= level.TotalEnemiesToSpawn)
            {
                _currentLevelIndex++;
                _totalSpawnedThisLevel = 0;
                return false;
            }
            if (_enemyPool.ActiveCount >= level.MaxEnemiesPerWave)
                return false;

            SpawnEnemy();
            _totalSpawnedThisLevel++;
            _totalSpawned++;
            return true;
        }

        private void SpawnEnemy()
        {
            Enemy enemy = _enemyPool.Get();
            enemy.transform.position = RandomPoint(spawnPositions).position;
            enemy.SetDestination(RandomPoint(attackPositions).position);
            enemy.SetTarget(_target);
        }

        private Transform RandomPoint(Transform[] points)
        {
            return points[Random.Range(0, points.Length)];
        }
    }
}