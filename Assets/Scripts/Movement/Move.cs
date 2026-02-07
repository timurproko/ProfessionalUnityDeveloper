using UnityEngine;

namespace ShootEmUp
{
    public sealed class Move
    {
        private readonly CharacterConfig _config;
        private readonly Rigidbody2D _rigidbody;

        public Move(CharacterConfig config, Rigidbody2D rigidbody)
        {
            _config = config;
            _rigidbody = rigidbody;
        }

        public void MoveToward(Vector2 direction, float deltaTime)
        {
            if (_rigidbody == null || _config == null)
                return;
            Vector2 step = direction * deltaTime * _config.Speed;
            _rigidbody.MovePosition(_rigidbody.position + step);
        }
    }
}
