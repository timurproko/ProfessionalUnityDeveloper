using System;
using UnityEngine;

namespace ShootEmUp
{
    public sealed class DamageableComponent : MonoBehaviour, IDamageable
    {
        public event Action<IDamageable, int> OnHealthChanged;
        public event Action<IDamageable> OnHealthEmpty;

        [SerializeField] private CharacterConfig config;
        [SerializeField] private Transform firePoint;
        [SerializeField] private Rigidbody2D rigidbody2d;

        private int health;

        public bool IsPlayer => config != null && config.IsPlayer;
        public float Speed => config != null ? config.Speed : 0f;
        public Transform FirePoint => firePoint;
        public Rigidbody2D Rigidbody2D => rigidbody2d;

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

        private void Awake()
        {
            if (config != null)
                health = config.DefaultHealth;
        }
    }
}
