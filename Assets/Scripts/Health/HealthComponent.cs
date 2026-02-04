using UnityEngine;

namespace ShootEmUp
{
    public sealed class HealthComponent : MonoBehaviour
    {
        public bool IsPlayer => _characterConfig != null && _characterConfig.IsPlayer;

        public int Health
        {
            get => _currentHealth;
            set => _currentHealth = value;
        }
        
        private CharacterConfig _characterConfig;
        
        [SerializeField, ReadOnly] private int _currentHealth;

        public void Init(CharacterConfig characterConfig)
        {
            _characterConfig = characterConfig;
            
            if (_characterConfig != null)
                _currentHealth = _characterConfig.DefaultHealth;
        }
    }
}