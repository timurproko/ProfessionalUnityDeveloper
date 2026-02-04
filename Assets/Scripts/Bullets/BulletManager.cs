using System.Collections.Generic;
using UnityEngine;

namespace ShootEmUp
{
    public sealed class BulletManager : MonoBehaviour
    {
        private const int PoolPrewarmCount = 10;

        [SerializeField] public Bullet _prefab;
        [SerializeField] public Transform _worldTransform;
        [SerializeField] private LevelBounds _levelBounds;
        [SerializeField] private Transform _container;

        private ObjectPool<Bullet> bulletPool;

        private void Awake()
        {
            bulletPool = new ObjectPool<Bullet>(
                _prefab,
                _container,
                _worldTransform,
                PoolPrewarmCount
            );

            FireRequestChannel.OnFireRequested += HandleFireRequest;
        }

        private void OnDestroy()
        {
            FireRequestChannel.OnFireRequested -= HandleFireRequest;
        }

        private void HandleFireRequest(FireRequest request)
        {
            if (request.Requester == null)
                return;

            BulletConfig config = request.Requester.BulletConfig;
            
            if (config == null)
                return;

            BulletSpawnRequest spawnRequest = config.CreateRequest(request.Position, request.Direction);
            
            SpawnBullet(
                spawnRequest.Position,
                spawnRequest.Velocity,
                spawnRequest.Color,
                spawnRequest.PhysicsLayer,
                spawnRequest.Damage,
                spawnRequest.IsPlayer
            );
        }

        private void FixedUpdate()
        {
            IReadOnlyList<Bullet> activeBullets = bulletPool.GetActiveSnapshot();
            for (int i = 0; i < activeBullets.Count; i++)
            {
                Bullet bullet = activeBullets[i];
                if (!_levelBounds.InBounds(bullet.transform.position))
                {
                    ReturnBullet(bullet);
                }
            }
        }

        private void SpawnBullet(
            Vector2 position, 
            Vector2 velocity, 
            Color color,
            int physicsLayer, 
            int damage, 
            bool isPlayer
        )
        {
            Bullet bullet = bulletPool.Get();
            bullet.Launch(position, physicsLayer, velocity, color, damage, isPlayer);
            bullet.OnCollisionEntered += OnBulletCollision;
        }

        private void OnBulletCollision(Bullet bullet, Collision2D collision)
        {
            DealDamage(bullet, collision.gameObject);
            ReturnBullet(bullet);
        }

        private void DealDamage(Bullet bullet, GameObject other)
        {
            int damage = bullet.Damage;
            if (damage <= 0)
                return;

            if (!other.TryGetComponent(out IDamageable damageable) || bullet.IsPlayer == damageable.IsPlayer)
                return;
            if (damageable.Health <= 0)
                return;

            damageable.Health = Mathf.Max(0, damageable.Health - damage);
        }

        private void ReturnBullet(Bullet bullet)
        {
            bullet.OnCollisionEntered -= OnBulletCollision;
            bulletPool.Return(bullet);
        }
    }
}