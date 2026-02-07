using UnityEngine;

namespace ShootEmUp
{
    [RequireComponent(typeof(HealthComponent))]
    [RequireComponent(typeof(AttackComponent))]
    [RequireComponent(typeof(TimedAttackComponent))]
    [RequireComponent(typeof(MoveComponent))]
    [RequireComponent(typeof(WaypointMoveComponent))]
    public sealed class Enemy : MonoBehaviour, IPoolable
    {
        [Space]
        [SerializeField] private HealthComponent _healthComponent;
        [SerializeField] private AttackComponent _attackComponent;
        [SerializeField] private TimedAttackComponent _timedAttack;
        [SerializeField] private MoveComponent _moveComponent;
        [SerializeField] private WaypointMoveComponent _waypointMove;
        [Space]
        [SerializeField] private Transform _firePoint;
        [SerializeField] private Rigidbody2D _rigidbody;
        [Space]
        [SerializeField] private CharacterConfig _characterConfig;
        [SerializeField] private BulletConfig _bulletConfig;

        public bool IsAlive => _healthComponent != null && _healthComponent.Health > 0;

        private void Awake()
        {
            _healthComponent?.Init(_characterConfig);
            _attackComponent?.Init(_bulletConfig, _firePoint);
            _timedAttack?.Init(_attackComponent);
            _moveComponent?.Init(_characterConfig, _rigidbody);
            _waypointMove?.Init(_moveComponent);
        }

        private void FixedUpdate()
        {
            if (_waypointMove.HasReachedDestination)
                _timedAttack?.SetEnabled(_attackComponent != null && _attackComponent.HasTarget);
        }

        public void SetTarget(ITarget target)
        {
            _attackComponent?.SetTarget(target);
        }

        public void SetDestination(Vector2 endPoint)
        {
            _waypointMove?.SetDestination(endPoint);
        }

        void IPoolable.OnGet()
        {
            Reset();
        }

        void IPoolable.OnReturn()
        {
            Reset();
        }

        private void Reset()
        {
            _healthComponent?.Reset();
            _attackComponent?.Reset();
            _timedAttack?.Reset();
            _waypointMove?.Reset();
        }
    }
}