using UnityEngine;

namespace ShootEmUp
{
    public sealed class PlayerController : MonoBehaviour
    {
        private Player _character;
        private bool _fireRequired;
        private float _moveDirection;

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
                _character.Fire();
                _fireRequired = false;
            }

            Vector2 moveDirection = new Vector2(_moveDirection, 0);
            _character.Move(moveDirection);
        }

        public void Init(Player character)
        {
            _character = character;
            _character.HealthComponent.OnHealthEmpty += _ => Time.timeScale = 0;
        }
    }
}