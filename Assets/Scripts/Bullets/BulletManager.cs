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
            this.bulletPool = new ObjectPool<Bullet>(
                this.prefab,
                this.container,
                this.worldTransform,
                PoolPrewarmCount
            );
        }

        private void FixedUpdate()
        {
            IReadOnlyList<Bullet> activeBullets = this.bulletPool.GetActiveSnapshot();
            for (int i = 0; i < activeBullets.Count; i++)
            {
                Bullet bullet = activeBullets[i];
                if (!this.levelBounds.InBounds(bullet.transform.position))
                {
                    this.ReturnBullet(bullet);
                }
            }
        }

        public void SpawnBullet(BulletSpawnRequest request)
        {
            this.SpawnBullet(
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
            Bullet bullet = this.bulletPool.Get();

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
            this.DealDamage(bullet, collision.gameObject);
            this.ReturnBullet(bullet);
        }

        private void ReturnBullet(Bullet bullet)
        {
            bullet.OnCollisionEntered -= this.OnBulletCollision;
            this.bulletPool.Return(bullet);
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