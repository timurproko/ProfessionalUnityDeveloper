using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ShootEmUp
{
    public sealed class EnemyManager : MonoBehaviour
    {
        private const int PoolPrewarmCount = 7;
        private const int MaxActiveEnemies = 5;

        [SerializeField] private Transform[] spawnPositions;
        [SerializeField] private Transform[] attackPositions;
        [SerializeField] private Player character;
        [SerializeField] private Transform worldTransform;
        [SerializeField] private Transform container;
        [SerializeField] private Enemy prefab;
        [SerializeField] private BulletManager bulletSystem;
        [SerializeField] private BulletConfig bulletConfig;

        private ObjectPool<Enemy> enemyPool;

        private void Awake()
        {
            this.enemyPool = new ObjectPool<Enemy>(
                this.prefab,
                this.container,
                this.worldTransform,
                PoolPrewarmCount
            );
        }

        private IEnumerator Start()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(1, 2));

                if (this.enemyPool.ActiveCount >= MaxActiveEnemies)
                    continue;

                Enemy enemy = this.enemyPool.Get();

                Transform spawnPosition = this.RandomPoint(this.spawnPositions);
                enemy.transform.position = spawnPosition.position;

                Transform attackPosition = this.RandomPoint(this.attackPositions);
                enemy.SetDestination(attackPosition.position);
                enemy.target = this.character;

                enemy.OnFire += this.OnFire;
            }
        }

        private void FixedUpdate()
        {
            foreach (Enemy enemy in this.enemyPool.GetActiveSnapshot())
            {
                if (enemy.HealthComponent.Health <= 0)
                {
                    enemy.OnFire -= this.OnFire;
                    this.enemyPool.Return(enemy);
                }
            }
        }

        private void OnFire(Vector2 position, Vector2 direction)
        {
            BulletSpawnRequest request = this.bulletConfig.CreateRequest(position, direction);
            this.bulletSystem.SpawnBullet(request);
        }

        private Transform RandomPoint(Transform[] points)
        {
            int index = Random.Range(0, points.Length);
            return points[index];
        }
    }
}