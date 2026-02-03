using UnityEngine;

namespace ShootEmUp
{
    public readonly struct BulletSpawnRequest
    {
        public Vector2 Position { get; }
        public Vector2 Velocity { get; }
        public Color Color { get; }
        public int PhysicsLayer { get; }
        public int Damage { get; }
        public bool IsPlayer { get; }

        public BulletSpawnRequest(
            Vector2 position,
            Vector2 velocity,
            Color color,
            int physicsLayer,
            int damage,
            bool isPlayer)
        {
            Position = position;
            Velocity = velocity;
            Color = color;
            PhysicsLayer = physicsLayer;
            Damage = damage;
            IsPlayer = isPlayer;
        }
    }
}
