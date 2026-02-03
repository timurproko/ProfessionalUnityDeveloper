using UnityEngine;

namespace ShootEmUp
{
    public sealed class PlayerController : MonoBehaviour
    {
        [SerializeField] private BulletManager _bulletManager;
        [SerializeField] private BulletConfig _bulletConfig;

        private Player _character;
        private bool _fireRequired;
        private float _moveDirection;
        
        public void Init(Player character)
        {
            _character = character;
            _character.HealthComponent.OnHealthEmpty += _ => Time.timeScale = 0;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) 
                _fireRequired = true;

            if (Input.GetKey(KeyCode.LeftArrow))
                _moveDirection = -1;
            else if (Input.GetKey(KeyCode.RightArrow))
                _moveDirection = 1;
            else
                _moveDirection = 0;
        }

        private void FixedUpdate()
        {
            if (_fireRequired)
            {
                Vector2 position = _character.FirePoint.position;
                Vector2 direction = _character.FirePoint.rotation * Vector3.up;
                BulletSpawnRequest request = _bulletConfig.CreateRequest(position, direction);
                _bulletManager.SpawnBullet(request);
                _fireRequired = false;
            }
            
            Vector2 moveDirection = new Vector2(this._moveDirection, 0);
            Vector2 moveStep = moveDirection * Time.fixedDeltaTime * _character.Speed;
            Vector2 targetPosition = _character.Rigidbody.position + moveStep;
            _character.Rigidbody.MovePosition(targetPosition);
        }
    }
}