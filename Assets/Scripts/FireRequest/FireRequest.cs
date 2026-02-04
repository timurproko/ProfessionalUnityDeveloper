using UnityEngine;

namespace ShootEmUp
{
    public readonly struct FireRequest
    {
        public Vector2 Position { get; }
        public Vector2 Velocity { get; }
        public Color Color { get; }
        public int PhysicsLayer { get; }
        public int Damage { get; }
        public bool IsPlayer { get; }

        public FireRequest(
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

        public static FireRequest Configure(BulletConfig config, Vector2 position, Vector2 direction)
        {
            Vector2 velocity = direction.normalized * config.Speed;
            return new FireRequest(
                position,
                velocity,
                config.Color,
                config.PhysicsLayer,
                config.Damage,
                config.IsPlayer
            );
        }
    }
}
