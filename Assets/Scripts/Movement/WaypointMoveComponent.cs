using System;
using UnityEngine;

namespace ShootEmUp
{
    [RequireComponent(typeof(MoveComponent))]
    public sealed class WaypointMoveComponent : MonoBehaviour
    {
        private const float ArrivalThreshold = 0.25f;

        public bool HasReachedDestination => _hasReached;

        private MoveComponent _moveComponent;
        private Vector2? _destination;
        private bool _hasReached;

        public void Init(MoveComponent moveComponent)
        {
            _moveComponent = moveComponent;
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
                return;
            }

            _moveComponent.Move(vector.normalized);
        }
    }
}
