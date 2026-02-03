using UnityEngine;

namespace ShootEmUp
{
    public readonly struct FireRequest
    {
        public IBulletProvider Requester { get; }
        public Vector2 Position { get; }
        public Vector2 Direction { get; }

        public FireRequest(IBulletProvider requester, Vector2 position, Vector2 direction)
        {
            Requester = requester;
            Position = position;
            Direction = direction;
        }
    }
}
