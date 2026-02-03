using System;
using UnityEngine;

namespace ShootEmUp
{
    [RequireComponent(typeof(DamageableComponent))]
    public sealed class Enemy : MonoBehaviour
    {
        public delegate void FireHandler(Vector2 position, Vector2 direction);
        public event FireHandler OnFire;

        [SerializeField] private DamageableComponent damageable;
        [SerializeField] private float countdown;
        [NonSerialized] public Player target;

        private Vector2 destination;
        private float currentTime;
        private bool isPointReached;

        public int Health { get => damageable.Health; set => damageable.Health = value; }
        public int health => damageable.Health;

        private void Awake()
        {
            if (this.damageable == null)
                this.damageable = this.GetComponent<DamageableComponent>();
        }

        public void Reset()
        {
            this.currentTime = this.countdown;
        }

        public void SetDestination(Vector2 endPoint)
        {
            this.destination = endPoint;
            this.isPointReached = false;
        }

        private void FixedUpdate()
        {
            if (this.isPointReached)
            {
                if (this.target != null && this.target.health <= 0)
                    return;

                this.currentTime -= Time.fixedDeltaTime;
                if (this.currentTime <= 0)
                {
                    Vector2 startPosition = this.damageable.FirePoint.position;
                    Vector2 vector = (Vector2)this.target.transform.position - startPosition;
                    Vector2 direction = vector.normalized;
                    this.OnFire?.Invoke(startPosition, direction);

                    this.currentTime += this.countdown;
                }
            }
            else
            {
                Vector2 vector = this.destination - (Vector2)this.transform.position;
                if (vector.magnitude <= 0.25f)
                {
                    this.isPointReached = true;
                    return;
                }

                Vector2 dir = vector.normalized * Time.fixedDeltaTime;
                Vector2 nextPosition = this.damageable.Rigidbody2D.position + dir * this.damageable.Speed;
                this.damageable.Rigidbody2D.MovePosition(nextPosition);
            }
        }
    }
}
