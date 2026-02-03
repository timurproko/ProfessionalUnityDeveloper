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

        public HealthComponent HealthComponent => _healthComponent;
        public BulletConfig BulletConfig => _bulletConfig;
        public Vector2 Position => transform.position;
        public float Speed => _characterConfig.Speed;
        public bool IsAlive => _healthComponent != null && _healthComponent.Health > 0;
        public Transform FirePoint => _firePoint;
        public Rigidbody2D Rigidbody => _rigidbody;

        private void Awake()
        {
            _healthComponent.Init(_characterConfig);
            _playerController.Init(this);
        }
    }
}