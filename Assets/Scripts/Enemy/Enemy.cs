using UnityEngine;

namespace ShootEmUp
{
    [RequireComponent(typeof(HealthComponent))]
    [RequireComponent(typeof(AttackComponent))]
    public sealed class Enemy : MonoBehaviour, IPoolable
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

        public bool IsAlive => _healthComponent != null && _healthComponent.Health > 0;

        private ITarget _target;
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

        private void FixedUpdate()
        {
            if (isPointReached)
            {
                _attackComponent?.SetActive(_target != null && _target.IsAlive);
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

        public void SetTarget(ITarget target)
        {
            _target = target;
        }

        public void SetDestination(Vector2 endPoint)
        {
            destination = endPoint;
            isPointReached = false;
            _attackComponent?.SetActive(false);
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
            _target = null;
            _attackComponent?.SetActive(false);
            _attackComponent?.Reset();
            isPointReached = false;
        }

        private void HandleAttackRequested()
        {
            if (_target == null || !_target.IsAlive)
                return;
            if (_bulletConfig == null)
                return;

            Vector2 startPosition = _firePoint.position;
            Vector2 vector = _target.Position - startPosition;
            Vector2 direction = vector.normalized;
            FireRequest request = FireRequest.Configure(_bulletConfig, startPosition, direction);
            FireRequestChannel.Raise(request);
        }
    }
}