using UnityEngine;

namespace ShootEmUp
{
    [RequireComponent(typeof(HealthComponent))]
    [RequireComponent(typeof(AttackComponent))]
    [RequireComponent(typeof(MovementComponent))]
    public sealed class Enemy : MonoBehaviour, IPoolable
    {
        [Space]
        [SerializeField] private HealthComponent _healthComponent;
        [SerializeField] private AttackComponent _attackComponent;
        [SerializeField] private MovementComponent _moveCoponent;
        [Space]
        [SerializeField] private Transform _firePoint;
        [SerializeField] private Rigidbody2D _rigidbody;
        [Space]
        [SerializeField] private CharacterConfig _characterConfig;
        [SerializeField] private BulletConfig _bulletConfig;

        public bool IsAlive => _healthComponent != null && _healthComponent.Health > 0;

        private ITarget _target;

        private void Awake()
        {
            _healthComponent.Init(_characterConfig);
            _attackComponent.Init(_bulletConfig, _firePoint);
            _moveCoponent.Init(_characterConfig, _rigidbody);

            _moveCoponent.OnDestinationReached += OnDestinationReached;
        }

        private void OnDestroy()
        {
            if (_moveCoponent != null)
                _moveCoponent.OnDestinationReached -= OnDestinationReached;
        }

        private void OnDestinationReached()
        {
            _attackComponent?.Reset();
        }

        private void FixedUpdate()
        {
            if (_moveCoponent.HasReachedDestination)
                Attack();
        }

        public void SetTarget(ITarget target)
        {
            _target = target;
            _attackComponent?.SetTarget(target);
        }

        public void SetDestination(Vector2 endPoint)
        {
            _moveCoponent.SetDestination(endPoint);
            _attackComponent?.SetCanFire(false);
        }

        private void Attack()
        {
            _attackComponent?.SetCanFire(_target != null && _target.IsAlive);
        }

        void IPoolable.OnGet()
        {
            _healthComponent?.Init(_characterConfig);
            _attackComponent?.Reset();
        }

        void IPoolable.OnReturn()
        {
            _target = null;
            _attackComponent?.SetTarget(null);
            _attackComponent?.SetCanFire(false);
            _attackComponent?.Reset();
            _moveCoponent?.Reset();
        }
    }
}