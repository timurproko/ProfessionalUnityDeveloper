using System;
using UnityEngine;

namespace ShootEmUp
{
    [RequireComponent(typeof(HealthComponent))]
    [RequireComponent(typeof(AttackComponent))]
    public sealed class Enemy : MonoBehaviour
    {
        [SerializeField] private HealthComponent _healthComponent;
        [SerializeField] private AttackComponent _attackComponent;
        [SerializeField] private CharacterConfig _characterConfig;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private Rigidbody2D _rigidbody;
        
        [NonSerialized] public Player target;
        
        public delegate void FireHandler(Vector2 position, Vector2 direction);
        public event FireHandler OnFire;
        public HealthComponent HealthComponent => _healthComponent;

        private Vector2 destination;
        private bool isPointReached;
        
        private void Awake()
        {
            _healthComponent.Init(_characterConfig);
            _attackComponent.OnFireRequested += HandleAttackRequested;
        }

        private void OnDestroy()
        {
            _attackComponent.OnFireRequested -= HandleAttackRequested;
        }

        public void Reset()
        {
            _attackComponent?.Reset();
        }

        public void SetDestination(Vector2 endPoint)
        {
            destination = endPoint;
            isPointReached = false;
            _attackComponent?.SetActive(false);
        }

        private void HandleAttackRequested()
        {
            if (!target || target.HealthComponent.Health <= 0)
                return;

            Vector2 startPosition = _firePoint.position;
            Vector2 vector = (Vector2)target.transform.position - startPosition;
            Vector2 direction = vector.normalized;
            OnFire?.Invoke(startPosition, direction);
        }

        private void FixedUpdate()
        {
            if (isPointReached)
            {
                _attackComponent?.SetActive(target && target.HealthComponent.Health > 0);
            }
            else
            {
                Vector2 vector = destination - (Vector2)transform.position;
                if (vector.magnitude <= 0.25f)
                {
                    isPointReached = true;
                    _attackComponent?.Reset();
                    return;
                }

                Vector2 dir = vector.normalized * Time.fixedDeltaTime;
                Vector2 nextPosition = _rigidbody.position + dir * _characterConfig.Speed;
                _rigidbody.MovePosition(nextPosition);
            }
        }
    }
}