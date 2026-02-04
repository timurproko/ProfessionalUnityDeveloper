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

        [Header("Debug")]
        [SerializeField, ReadOnly] private int _totalSpawned;
        [SerializeField, ReadOnly] private int _currentLevel;
        [SerializeField, ReadOnly] private int _remainingToSpawn;

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
            RefreshDebugFields();
        }

        private void RefreshDebugFields()
        {
            _currentLevel = _currentLevelIndex + 1;
            LevelConfig level = _levelsConfig != null ? _levelsConfig.GetLevel(_currentLevelIndex) : null;
            _remainingToSpawn = level != null
                ? Mathf.Max(0, level.TotalEnemiesToSpawn - _totalSpawnedThisLevel)
                : 0;
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
            if (!TryGetAvailableAttackPosition(out int attackIndex))
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

        private bool TryGetAvailableAttackPosition(out int index)
        {
            index = -1;
            if (attackPositions == null || attackPositions.Length == 0)
                return false;

            int availableCount = 0;
            for (int i = 0; i < attackPositions.Length; i++)
            {
                if (attackPositions[i] != null && !IsAttackPositionOccupied(i))
                    availableCount++;
            }

            if (availableCount == 0)
                return false;

            int pick = Random.Range(0, availableCount);
            for (int i = 0; i < attackPositions.Length; i++)
            {
                if (attackPositions[i] == null || IsAttackPositionOccupied(i))
                    continue;
                if (pick-- == 0)
                {
                    index = i;
                    return true;
                }
            }

            return false;
        }

        private bool IsAttackPositionOccupied(int positionIndex)
        {
            foreach (int occupiedIndex in _attackPositionIndex.Values)
            {
                if (occupiedIndex == positionIndex)
                    return true;
            }
            return false;
        }

        private Transform RandomPoint(Transform[] points)
        {
            return points[Random.Range(0, points.Length)];
        }
    }
}