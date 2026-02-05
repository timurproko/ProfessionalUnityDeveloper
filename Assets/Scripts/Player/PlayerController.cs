using UnityEngine;

namespace ShootEmUp
{
    public sealed class PlayerController : MonoBehaviour
    {
        private Player _character;
        private bool _fireRequired;
        private float _moveDirection;

        public void Init(Player character)
        {
            _character = character;
        }

        private void Update()
        {
            if (_character != null && !_character.IsAlive)
            {
                Time.timeScale = 0;
                return;
            }

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
    }
}