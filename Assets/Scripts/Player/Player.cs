using UnityEngine;

namespace ShootEmUp
{
    [RequireComponent(typeof(HealthComponent))]
    [RequireComponent(typeof(AttackComponent))]
    [RequireComponent(typeof(MoveComponent))]
    public sealed class Player : MonoBehaviour, ITarget
    {
        [Space]
        [SerializeField] private HealthComponent _healthComponent;
        [SerializeField] private AttackComponent _attackComponent;
        [SerializeField] private MoveComponent _moveComponent;
        [SerializeField] private PlayerController _playerController;
        [Space]
        [SerializeField] private Transform _firePoint;
        [SerializeField] private Rigidbody2D _rigidbody;
        [Space]
        [SerializeField] private CharacterConfig _characterConfig;
        [SerializeField] private BulletConfig _bulletConfig;

        public Vector2 Position => transform.position;
        public bool IsAlive => _healthComponent != null && _healthComponent.Health > 0;

        private void Awake()
        {
            _healthComponent?.Init(_characterConfig);
            _attackComponent?.Init(_bulletConfig, _firePoint);
            _moveComponent?.Init(_characterConfig, _rigidbody);
            _playerController?.Init(this);
        }

        public void Fire()
        {
            _attackComponent?.Fire();
        }

        public void Move(Vector2 direction)
        {
            _moveComponent?.Move(direction);
        }
    }
}