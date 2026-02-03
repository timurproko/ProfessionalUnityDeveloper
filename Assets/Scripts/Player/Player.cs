using System;
using UnityEngine;

namespace ShootEmUp
{
    [RequireComponent(typeof(DamageableComponent))]
    public sealed class Player : MonoBehaviour
    {
        [SerializeField] private DamageableComponent damageable;

        public Transform firePoint => damageable.FirePoint;
        public Rigidbody2D _rigidbody => damageable.Rigidbody2D;
        public float speed => damageable.Speed;
        public int health => damageable.Health;

        public event Action<IDamageable, int> OnHealthChanged
        {
            add => damageable.OnHealthChanged += value;
            remove => damageable.OnHealthChanged -= value;
        }
        public event Action<IDamageable> OnHealthEmpty
        {
            add => damageable.OnHealthEmpty += value;
            remove => damageable.OnHealthEmpty -= value;
        }
    }
}
