using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ShootEmUp
{
    public sealed class EnemyManager : MonoBehaviour
    {
        [Header("Setup")]
        [SerializeField] private Transform[] spawnPositions;
        [SerializeField] private Transform[] attackPositions;
        [SerializeField] private Player _target;
        [SerializeField] private Transform _worldTransform;
        [SerializeField] private Transform _container;
        [SerializeField] private Enemy _prefab;
        [SerializeField] private LevelConfig _levelConfig;

        private const int PoolPrewarmCount = 5;
        private const float SpawnDelayMin = 1f;
        private const float SpawnDelayMax = 2f;

        private readonly Dictionary<Enemy, int> _enemyToAttackIndex = new();
        private bool[] _occupiedAttackSlots;

        private ObjectPool<Enemy> _enemyPool;
        private int _totalSpawned;
        private float _nextSpawnTime;

        private void Awake()
        {
            _enemyPool = new ObjectPool<Enemy>(_prefab, _container, _worldTransform, PoolPrewarmCount);
            _occupiedAttackSlots = new bool[attackPositions?.Length ?? 0];
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
                if (enemy.IsAlive)
                    continue;

                if (_enemyToAttackIndex.Remove(enemy, out int slot))
                {
                    if (slot >= 0 && slot < _occupiedAttackSlots.Length)
                        _occupiedAttackSlots[slot] = false;
                }

                _enemyPool.Return(enemy);
            }
        }

        private bool TrySpawnEnemy()
        {
            if (_levelConfig == null)
                return false;
            if (Time.time < _nextSpawnTime)
                return false;
            if (_totalSpawned >= _levelConfig.TotalEnemiesToSpawn)
                return false;
            if (_enemyPool.ActiveCount >= _levelConfig.MaxEnemiesPerWave)
                return false;
            if (!TryPickFreeAttackSlot(out int attackIndex))
                return false;

            SpawnEnemy(attackIndex);
            _totalSpawned++;
            return true;
        }

        private void SpawnEnemy(int attackIndex)
        {
            Enemy enemy = _enemyPool.Get();

            enemy.transform.position = RandomPoint(spawnPositions).position;
            enemy.SetDestination(attackPositions[attackIndex].position);
            enemy.SetTarget(_target);

            _enemyToAttackIndex[enemy] = attackIndex;
            _occupiedAttackSlots[attackIndex] = true;
        }

        private bool TryPickFreeAttackSlot(out int index)
        {
            index = -1;
            if (attackPositions == null || attackPositions.Length == 0)
                return false;

            int freeCount = 0;
            for (int i = 0; i < attackPositions.Length; i++)
            {
                if (attackPositions[i] != null && !_occupiedAttackSlots[i])
                    freeCount++;
            }

            if (freeCount == 0)
                return false;

            int pick = Random.Range(0, freeCount);
            for (int i = 0; i < attackPositions.Length; i++)
            {
                if (attackPositions[i] == null || _occupiedAttackSlots[i])
                    continue;

                if (pick-- == 0)
                {
                    index = i;
                    return true;
                }
            }

            return false;
        }

        private Transform RandomPoint(Transform[] points)
        {
            return points[Random.Range(0, points.Length)];
        }
    }
}
