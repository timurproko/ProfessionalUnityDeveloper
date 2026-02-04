using System;
using UnityEngine;

namespace ShootEmUp
{
    public sealed class HealthComponent : MonoBehaviour
    {
        public event Action OnHealthEmpty;
        
        public bool IsPlayer => _characterConfig != null && _characterConfig.IsPlayer;

        public int Health
        {
            get => _currentHealth;
            set
            {
                _currentHealth = value;
                if (_currentHealth <= 0)
                    OnHealthEmpty?.Invoke();
            }
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