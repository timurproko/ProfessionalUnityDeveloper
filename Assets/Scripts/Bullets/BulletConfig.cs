using UnityEngine;

namespace ShootEmUp
{
    [CreateAssetMenu(fileName = "BulletConfig", menuName = "ShootEmUp/Bullet Config", order = 1)]
    public sealed class BulletConfig : ScriptableObject
    {
        [SerializeField] private bool _isPlayer;
        [SerializeField] private Color _color = Color.white;
        [SerializeField, Layer] private int _physicsLayer;
        [SerializeField] private int _damage = 1;
        [SerializeField] private float _speed = 3f;
        
        public BulletSpawnRequest CreateRequest(Vector2 position, Vector2 direction)
        {
            Vector2 velocity = direction.normalized * _speed;
            return new BulletSpawnRequest(
                position,
                velocity,
                _color,
                _physicsLayer,
                _damage,
                _isPlayer
            );
        }
    }
}
