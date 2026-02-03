using System;
using UnityEngine;

namespace ShootEmUp
{
    public sealed class Player : MonoBehaviour, IDamageable
    {
        public event Action<IDamageable, int> OnHealthChanged;
        public event Action<IDamageable> OnHealthEmpty;

        public bool IsPlayer => isPlayer;
        public int Health
        {
            get => health;
            set
            {
                health = value;
                OnHealthChanged?.Invoke(this, health);
                if (health <= 0)
                    OnHealthEmpty?.Invoke(this);
            }
        }

        [SerializeField] public bool isPlayer;
        [SerializeField] public Transform firePoint;
        [SerializeField] public int health;
        [SerializeField] public Rigidbody2D _rigidbody;
        [SerializeField] public float speed = 5.0f;
    }
}