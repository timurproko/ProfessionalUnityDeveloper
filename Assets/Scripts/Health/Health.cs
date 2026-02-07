using UnityEngine;

namespace ShootEmUp
{
    public sealed class Health
    {
        private readonly CharacterConfig _config;
        private int _currentHealth;

        public Health(CharacterConfig config)
        {
            _config = config;
            _currentHealth = config != null ? config.DefaultHealth : 0;
        }

        public bool IsPlayer => _config != null && _config.IsPlayer;

        public int CurrentHealth
        {
            get => _currentHealth;
            set => _currentHealth = Mathf.Max(0, value);
        }

        public void Reset()
        {
            if (_config != null)
                _currentHealth = _config.DefaultHealth;
        }
    }
}
