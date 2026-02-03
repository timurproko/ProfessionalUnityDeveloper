using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShootEmUp
{
    public sealed class BulletManager : MonoBehaviour
    {
        private const int PoolPrewarmCount = 10;

        [SerializeField] public Bullet prefab;
        [SerializeField] public Transform worldTransform;
        [SerializeField] private LevelBounds levelBounds;
        [SerializeField] private Transform container;

        private ObjectPool<Bullet> bulletPool;

        private void Awake()
        {
            bulletPool = new ObjectPool<Bullet>(
                prefab,
                container,
                worldTransform,
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

            BulletConfig config = request.Requester.GetBulletConfig();
            if (config == null)
                return;

            BulletSpawnRequest spawnRequest = config.CreateRequest(request.Position, request.Direction);
            SpawnBullet(spawnRequest);
        }

        private void FixedUpdate()
        {
            IReadOnlyList<Bullet> activeBullets = bulletPool.GetActiveSnapshot();
            for (int i = 0; i < activeBullets.Count; i++)
            {
                Bullet bullet = activeBullets[i];
                if (!levelBounds.InBounds(bullet.transform.position))
                {
                    ReturnBullet(bullet);
                }
            }
        }

        public void SpawnBullet(BulletSpawnRequest request)
        {
            SpawnBullet(
                request.Position,
                request.Velocity,
                request.Color,
                request.PhysicsLayer,
                request.Damage,
                request.IsPlayer
            );
        }

        public void SpawnBullet(
            Vector2 position,
            Vector2 velocity,
            Color color,
            int physicsLayer,
            int damage,
            bool isPlayer
        )
        {
            Bullet bullet = bulletPool.Get();

            bullet.transform.position = position;
            bullet.spriteRenderer.color = color;
            bullet.gameObject.layer = physicsLayer;
            bullet.damage = damage;
            bullet.isPlayer = isPlayer;
            bullet.rigidbody2D.linearVelocity = velocity;

            bullet.OnCollisionEntered += this.OnBulletCollision;
        }

        private void OnBulletCollision(Bullet bullet, Collision2D collision)
        {
            DealDamage(bullet, collision.gameObject);
            ReturnBullet(bullet);
        }

        private void ReturnBullet(Bullet bullet)
        {
            bullet.OnCollisionEntered -= this.OnBulletCollision;
            bulletPool.Return(bullet);
        }

        private void DealDamage(Bullet bullet, GameObject other)
        {
            int damage = bullet.damage;
            if (damage <= 0)
                return;

            if (!other.TryGetComponent(out IDamageable damageable) || bullet.isPlayer == damageable.IsPlayer)
                return;
            if (damageable.Health <= 0)
                return;

            damageable.Health = Mathf.Max(0, damageable.Health - damage);
        }
    }
}