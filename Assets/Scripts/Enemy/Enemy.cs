using UnityEngine;

namespace ShootEmUp
{
    public sealed class Enemy : MonoBehaviour, IPoolable, IDamageable
    {
        [Space]
        [SerializeField] private Transform _firePoint;
        [SerializeField] private Rigidbody2D _rigidbody;
        [Space]
        [SerializeField] private CharacterConfig _characterConfig;
        [SerializeField] private BulletConfig _bulletConfig;
        [Space]
        [SerializeField] private float _attackInterval = 1f;

        private Health _health;
        private TimedAttack _timedAttack;
        private WaypointMove _waypointMove;

        public bool IsAlive => _health != null && _health.CurrentHealth > 0;

        public int Health
        {
            get => _health?.CurrentHealth ?? 0;
            set { if (_health != null) _health.CurrentHealth = value; }
        }

        public bool IsPlayer => _health != null && _health.IsPlayer;

        private void Awake()
        {
            _health = new Health(_characterConfig);
            _timedAttack = new TimedAttack(_bulletConfig, _firePoint, _attackInterval);
            _waypointMove = new WaypointMove(_characterConfig, _rigidbody, transform);
        }

        private void FixedUpdate()
        {
            _waypointMove.Tick(Time.fixedDeltaTime);

            if (_waypointMove.HasReachedDestination)
                _timedAttack.SetEnabled(_timedAttack.HasTarget);

            _timedAttack.Tick(Time.fixedDeltaTime);
        }

        public void SetTarget(ITarget target)
        {
            _timedAttack.SetTarget(target);
        }

        public void SetDestination(Vector2 endPoint)
        {
            _waypointMove.SetDestination(endPoint);
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
            _health?.Reset();
            _timedAttack?.Reset();
            _waypointMove?.Reset();
        }
    }
}
