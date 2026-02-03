using UnityEngine;

namespace ShootEmUp
{
    [CreateAssetMenu(fileName = "CharacterConfig", menuName = "ShootEmUp/Character Config", order = 0)]
    public sealed class CharacterConfig : ScriptableObject
    {
        [SerializeField] private bool _isPlayer;
        [SerializeField] private int _defaultHealth = 1;
        [SerializeField] private float _speed = 5f;

        public int DefaultHealth => _defaultHealth;
        public bool IsPlayer => _isPlayer;
        public float Speed => _speed;
    }
}
