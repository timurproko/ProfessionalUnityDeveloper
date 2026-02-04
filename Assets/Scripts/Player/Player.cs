using UnityEngine;

namespace ShootEmUp
{
    [RequireComponent(typeof(HealthComponent))]
    public sealed class Player : MonoBehaviour, ITarget
    {
        [Space]
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private HealthComponent _healthComponent;
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
            _healthComponent.Init(_characterConfig);
            _playerController.Init(this);
        }

        public void Fire()
        {
            if (_bulletConfig == null)
                return;
            
            Vector2 position = _firePoint.position;
            Vector2 direction = _firePoint.rotation * Vector3.up;
            FireRequest request = FireRequest.Configure(_bulletConfig, position, direction);
            FireRequestChannel.Raise(request);
        }

        public void Move(Vector2 direction)
        {
            Vector2 moveStep = direction * Time.fixedDeltaTime * _characterConfig.Speed;
            Vector2 targetPosition = _rigidbody.position + moveStep;
            _rigidbody.MovePosition(targetPosition);
        }
    }
}