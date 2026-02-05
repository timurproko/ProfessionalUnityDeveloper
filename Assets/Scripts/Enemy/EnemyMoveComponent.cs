using System;
using UnityEngine;

namespace ShootEmUp
{
    public sealed class EnemyMoveComponent : MonoBehaviour
    {
        private const float ArrivalThreshold = 0.25f;

        public event Action OnDestinationReached;
        public bool HasReachedDestination => _hasReached;

        private CharacterConfig _characterConfig;
        private Rigidbody2D _rigidbody;
        private Vector2? _destination;
        private bool _hasReached;

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
            _destination = null;
            _hasReached = false;
        }

        private void FixedUpdate()
        {
            if (!_destination.HasValue || _hasReached)
                return;

            Vector2 vector = _destination.Value - (Vector2)transform.position;
            if (vector.magnitude <= ArrivalThreshold)
            {
                _hasReached = true;
                _destination = null;
                OnDestinationReached?.Invoke();
                return;
            }

            Move(vector.normalized);
        }

        private void Move(Vector2 direction)
        {
            Vector2 moveStep = direction * Time.fixedDeltaTime * _characterConfig.Speed;
            _rigidbody.MovePosition(_rigidbody.position + moveStep);
        }
    }
}
