using UnityEngine;

namespace ShootEmUp
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class MoveComponent : MonoBehaviour
    {
        private CharacterConfig _characterConfig;
        private Rigidbody2D _rigidbody;

        public void Init(CharacterConfig characterConfig, Rigidbody2D rigidbody)
        {
            _characterConfig = characterConfig;
            _rigidbody = rigidbody;
        }

        public void Move(Vector2 direction)
        {
            Vector2 moveStep = direction * Time.fixedDeltaTime * _characterConfig.Speed;
            _rigidbody.MovePosition(_rigidbody.position + moveStep);
        }
    }
}
