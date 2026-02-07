using UnityEngine;

namespace ShootEmUp
{
    public sealed class WaypointMove
    {
        private const float ArrivalThreshold = 0.25f;

        private readonly Move _move;
        private readonly Transform _transform;
        private Vector2? _destination;
        private bool _hasReached;

        public bool HasReachedDestination => _hasReached;

        public WaypointMove(CharacterConfig config, Rigidbody2D rigidbody, Transform transform)
        {
            _move = new Move(config, rigidbody);
            _transform = transform;
        }

        public void SetDestination(Vector2 destination)
        {
            _destination = destination;
            _hasReached = false;
        }

        public void Tick(float deltaTime)
        {
            if (!_destination.HasValue || _hasReached)
                return;

            Vector2 position = _transform.position;
            Vector2 vector = _destination.Value - position;
            if (vector.magnitude <= ArrivalThreshold)
            {
                _hasReached = true;
                _destination = null;
                return;
            }

            _move.MoveToward(vector.normalized, deltaTime);
        }

        public void Reset()
        {
            _destination = null;
            _hasReached = false;
        }
    }
}
