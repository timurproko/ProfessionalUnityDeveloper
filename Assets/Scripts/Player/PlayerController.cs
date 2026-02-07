using UnityEngine;

namespace ShootEmUp
{
    public sealed class PlayerController
    {
        private bool _fireRequired;
        private float _moveDirection;

        public void ReadInput()
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

        public bool ConsumeFire()
        {
            bool v = _fireRequired;
            _fireRequired = false;
            return v;
        }

        public float MoveDirection => _moveDirection;
    }
}
