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

        public bool IsPlayer => _isPlayer;
        public Color Color => _color;
        public int PhysicsLayer => _physicsLayer;
        public int Damage => _damage;
        public float Speed => _speed;
    }
}
