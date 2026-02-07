using UnityEngine;

namespace ShootEmUp
{
    [RequireComponent(typeof(HealthComponent))]
    [RequireComponent(typeof(MoveComponent))]
    public sealed class Player : MonoBehaviour, ITarget
    {
        [Space]
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private HealthComponent _healthComponent;
        [SerializeField] private MoveComponent _moveComponent;
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
            _moveComponent?.Init(_characterConfig, _rigidbody);
            _playerController?.Init(this);
        }

        public void Fire()
        {
            if (_bulletConfig == null)
                return;

            Vector2 position = _firePoint.position;
            Vector2 direction = _firePoint.rotation * Vector3.up;
            FireRequestService.RequestFire(_bulletConfig, position, direction);
        }

        public void Move(Vector2 direction)
        {
            _moveComponent?.Move(direction);
        }
    }
}