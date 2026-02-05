using System;
using UnityEngine;

namespace ShootEmUp
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class MovementComponent : MonoBehaviour
    {
        private const float ArrivalThreshold = 0.25f;

        public event Action OnDestinationReached;

        private CharacterConfig _characterConfig;
        private Rigidbody2D _rigidbody;
        private Vector2 _destination;
        private bool _hasReached;

        public bool HasReachedDestination => _hasReached;

        public void Init(CharacterConfig characterConfig, Rigidbody2D rigidbody)
        {
            _characterConfig = characterConfig;
            _rigidbody = rigidbody;
        }

        public void SetDestination(Vector2 destination)
        {
            _destination = destination;
            _hasReached = false;
        }

        public void Reset()
        {
            _hasReached = false;
        }

        private void FixedUpdate()
        {
            if (_hasReached)
                return;

            Vector2 vector = _destination - (Vector2)transform.position;
            if (vector.magnitude <= ArrivalThreshold)
            {
                _hasReached = true;
                OnDestinationReached?.Invoke();
                return;
            }

            Vector2 direction = vector.normalized;
            Vector2 moveStep = direction * Time.fixedDeltaTime * _characterConfig.Speed;
            _rigidbody.MovePosition(_rigidbody.position + moveStep);
        }
    }
}
