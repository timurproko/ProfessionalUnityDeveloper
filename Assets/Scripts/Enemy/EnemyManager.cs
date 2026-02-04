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

        private readonly Dictionary<Enemy, int> _attackPositionIndex = new();
        
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
                {
                    _attackPositionIndex.Remove(enemy);
                    _enemyPool.Return(enemy);
                }
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
            if (!TryGetAvailableAttackPositionIndex(out int attackIndex))
                return false;

            SpawnEnemy(attackIndex);
            _totalSpawnedThisLevel++;
            _totalSpawned++;
            return true;
        }

        private void SpawnEnemy(int attackPositionIndex)
        {
            Enemy enemy = _enemyPool.Get();
            enemy.transform.position = RandomPoint(spawnPositions).position;
            enemy.SetDestination(attackPositions[attackPositionIndex].position);
            enemy.SetTarget(_target);
            _attackPositionIndex[enemy] = attackPositionIndex;
        }

        private bool TryGetAvailableAttackPositionIndex(out int index)
        {
            index = -1;
            if (attackPositions == null || attackPositions.Length == 0)
                return false;

            var occupied = new HashSet<int>(_attackPositionIndex.Values);
            var available = new List<int>();
            for (int i = 0; i < attackPositions.Length; i++)
            {
                if (attackPositions[i] != null && !occupied.Contains(i))
                    available.Add(i);
            }

            if (available.Count == 0)
                return false;

            index = available[Random.Range(0, available.Count)];
            return true;
        }

        private Transform RandomPoint(Transform[] points)
        {
            return points[Random.Range(0, points.Length)];
        }
    }
}