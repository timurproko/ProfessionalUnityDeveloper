using System;
using UnityEngine;

namespace ShootEmUp
{
    public sealed class Bullet : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        public event Action<Bullet, Collision2D> OnCollisionEntered;

        public int Damage => _damage;
        public bool IsPlayer => _isPlayer;

        private bool _isPlayer;
        private int _damage;

        public void Launch(Vector2 position, int physicsLayer, Vector2 velocity, Color color, int damage, bool isPlayer)
        {
            transform.position = position;
            gameObject.layer = physicsLayer;
            _rigidbody.linearVelocity = velocity;
            _spriteRenderer.color = color;
            _damage = damage;
            _isPlayer = isPlayer;
        }
        
        private void OnCollisionEnter2D(Collision2D collision)
        {
            OnCollisionEntered?.Invoke(this, collision);
        }
    }
}