using UnityEngine;

namespace ShootEmUp
{
    [RequireComponent(typeof(AttackComponent))]
    public sealed class TimedAttackComponent : MonoBehaviour
    {
        [SerializeField] private float _attackInterval = 1f;

        private AttackComponent _attackComponent;
        private float _currentTime;
        private bool _enabled;

        private void Awake()
        {
            _currentTime = _attackInterval;
        }

        public void Init(AttackComponent attackComponent)
        {
            _attackComponent = attackComponent;
        }

        private void FixedUpdate()
        {
            if (!_enabled)
                return;

            _currentTime -= Time.fixedDeltaTime;
            if (_currentTime <= 0f)
            {
                _attackComponent.Fire();
                _currentTime += _attackInterval;
            }
        }

        public void Reset()
        {
            SetEnabled(false);
            ResetTimer();
        }

        public void SetEnabled(bool enabled)
        {
            _enabled = enabled;
        }

        public void ResetTimer()
        {
            _currentTime = _attackInterval;
        }
    }
}
