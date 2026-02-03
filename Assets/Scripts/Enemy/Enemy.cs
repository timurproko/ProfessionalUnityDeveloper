using System;
using UnityEngine;

namespace ShootEmUp
{
    [RequireComponent(typeof(HealthComponent))]
    [RequireComponent(typeof(AttackComponent))]
    public sealed class Enemy : MonoBehaviour, IPoolable, IBulletProvider
    {
        [Space]
        [SerializeField] private HealthComponent _healthComponent;
        [SerializeField] private AttackComponent _attackComponent;
        [Space]
        [SerializeField] private Transform _firePoint;
        [SerializeField] private Rigidbody2D _rigidbody;
        [Space]
        [SerializeField] private CharacterConfig _characterConfig;
        [SerializeField] private BulletConfig _bulletConfig;
        
        [NonSerialized] public ITarget Target;
        
        public HealthComponent HealthComponent => _healthComponent;
        BulletConfig IBulletProvider.GetBulletConfig() => _bulletConfig;

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

        void IPoolable.OnGet()
        {
            _healthComponent?.Init(_characterConfig);
            _attackComponent?.Reset();
        }

        void IPoolable.OnReturn()
        {
            Target = null;
            _attackComponent?.SetActive(false);
            _attackComponent?.Reset();
            isPointReached = false;
        }

        public void SetDestination(Vector2 endPoint)
        {
            destination = endPoint;
            isPointReached = false;
            _attackComponent?.SetActive(false);
        }

        private void HandleAttackRequested()
        {
            if (Target == null || (Target as UnityEngine.Object) == null || !Target.IsAlive)
                return;
            if (_bulletConfig == null)
                return;

            Vector2 startPosition = _firePoint.position;
            Vector2 vector = Target.Position - startPosition;
            Vector2 direction = vector.normalized;
            FireRequestChannel.Raise(this, startPosition, direction);
        }

        private void FixedUpdate()
        {
            if (isPointReached)
            {
                _attackComponent?.SetActive(Target != null && (Target as UnityEngine.Object) != null && Target.IsAlive);
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