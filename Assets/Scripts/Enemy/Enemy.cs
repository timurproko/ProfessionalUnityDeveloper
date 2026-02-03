using System;
using UnityEngine;

namespace ShootEmUp
{
    [RequireComponent(typeof(DamageableComponent))]
    [RequireComponent(typeof(AttackCountdownComponent))]
    public sealed class Enemy : MonoBehaviour
    {
        public delegate void FireHandler(Vector2 position, Vector2 direction);

        public event FireHandler OnFire;

        [SerializeField] private DamageableComponent damageable;
        [SerializeField] private AttackCountdownComponent attackCountdown;
        [NonSerialized] public Player target;

        private Vector2 destination;
        private bool isPointReached;

        public int Health
        {
            get => damageable.Health;
            set => damageable.Health = value;
        }

        private void Awake()
        {
            if (this.attackCountdown != null)
                this.attackCountdown.OnFireRequested += this.HandleAttackRequested;
        }

        private void OnDestroy()
        {
            if (this.attackCountdown != null)
                this.attackCountdown.OnFireRequested -= this.HandleAttackRequested;
        }

        public void Reset()
        {
            this.attackCountdown?.Reset();
        }

        public void SetDestination(Vector2 endPoint)
        {
            this.destination = endPoint;
            this.isPointReached = false;
            this.attackCountdown?.SetActive(false);
        }

        private void HandleAttackRequested()
        {
            if (!this.target || this.target.health <= 0)
                return;

            Vector2 startPosition = this.damageable.FirePoint.position;
            Vector2 vector = (Vector2)this.target.transform.position - startPosition;
            Vector2 direction = vector.normalized;
            this.OnFire?.Invoke(startPosition, direction);
        }

        private void FixedUpdate()
        {
            if (this.isPointReached)
            {
                this.attackCountdown?.SetActive(this.target != null && this.target.health > 0);
            }
            else
            {
                Vector2 vector = this.destination - (Vector2)this.transform.position;
                if (vector.magnitude <= 0.25f)
                {
                    this.isPointReached = true;
                    this.attackCountdown?.Reset();
                    return;
                }

                Vector2 dir = vector.normalized * Time.fixedDeltaTime;
                Vector2 nextPosition = this.damageable.Rigidbody2D.position + dir * this.damageable.Speed;
                this.damageable.Rigidbody2D.MovePosition(nextPosition);
            }
        }
    }
}