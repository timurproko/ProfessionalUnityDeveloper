using UnityEngine;

namespace ShootEmUp
{
    [RequireComponent(typeof(HealthComponent))]
    public sealed class Player : MonoBehaviour
    {
        [SerializeField] private PlayerController _playerController;
        [SerializeField] private HealthComponent _healthComponent;
        [SerializeField] private CharacterConfig _characterConfig;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private Rigidbody2D _rigidbody;

        public HealthComponent HealthComponent => _healthComponent;
        
        public Transform FirePoint => _firePoint;
        public Rigidbody2D Rigidbody => _rigidbody;
        public float Speed => _characterConfig.Speed;

        private void Awake()
        {
            _healthComponent.Init(_characterConfig);
            _playerController.Init(this);
        }
    }
}