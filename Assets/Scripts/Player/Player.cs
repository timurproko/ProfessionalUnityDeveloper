using UnityEngine;

namespace ShootEmUp
{
    public sealed class Player : MonoBehaviour, IDamageable
    {
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
        private PlayerController _controller;

        private void Awake()
        {
            _health = new Health(_characterConfig);
            _move = new Move(_characterConfig, _rigidbody);
            _attack = new Attack(_bulletConfig, _firePoint);
            _controller = new PlayerController();
        }

        private void Update()
        {
            if (!IsAlive)
            {
                Time.timeScale = 0;
                return;
            }

            _controller.ReadInput();
        }

        private void FixedUpdate()
        {
            if (_controller.ConsumeFire())
                _attack?.Fire();

            Move(_controller.MoveDirection);
        }

        private void Move(float direction)
        {
            _move?.MoveToward(new Vector2(direction, 0), Time.fixedDeltaTime);
        }
    }
}