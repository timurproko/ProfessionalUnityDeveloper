using UnityEngine;

namespace ShootEmUp
{
    public sealed class Player : MonoBehaviour, IDamageable
    {
        [Space]
        [SerializeField] private PlayerController _playerController;
        [Space]
        [SerializeField] private Transform _firePoint;
        [SerializeField] private Rigidbody2D _rigidbody;
        [Space]
        [SerializeField] private CharacterConfig _characterConfig;
        [SerializeField] private BulletConfig _bulletConfig;

        public bool IsPlayer => _characterConfig != null && _characterConfig.IsPlayer;
        public bool IsAlive => _health != null && _health.CurrentHealth > 0;
        public int Health { get => _health.CurrentHealth; set => _health.CurrentHealth = value; }

        private Health _health;
        private Move _move;
        private Attack _attack;
        
        private void Awake()
        {
            _health = new Health(_characterConfig);
            _move = new Move(_characterConfig, _rigidbody);
            _attack = new Attack(_bulletConfig, _firePoint);
            
            _playerController?.Init(this);
        }

        public void Fire()
        {
            _attack?.Fire();
        }

        public void Move(Vector2 direction)
        {
            _move?.MoveToward(direction, Time.fixedDeltaTime);
        }
    }
}
