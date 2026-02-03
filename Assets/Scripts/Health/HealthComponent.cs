using System;
using UnityEngine;

namespace ShootEmUp
{
    public sealed class HealthComponent : MonoBehaviour, IDamageable
    {
        public event Action<IDamageable, int> OnHealthChanged;
        public event Action<IDamageable> OnHealthEmpty;
        
        public bool IsPlayer => _characterConfig != null && _characterConfig.IsPlayer;

        public int Health
        {
            get => _health;
            set
            {
                _health = value;
                OnHealthChanged?.Invoke(this, _health);
                if (_health <= 0)
                    OnHealthEmpty?.Invoke(this);
            }
        }
        
        private CharacterConfig _characterConfig;
        [SerializeField, ReadOnly] private int _health;

        public void Init(CharacterConfig characterConfig)
        {
            _characterConfig = characterConfig;
            
            if (_characterConfig != null)
                _health = _characterConfig.DefaultHealth;
        }
    }
}