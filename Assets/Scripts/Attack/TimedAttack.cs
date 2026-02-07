using UnityEngine;

namespace ShootEmUp
{
    public sealed class TimedAttack
    {
        private readonly Attack _attack;
        private readonly float _interval;
        private float _currentTime;
        private bool _enabled;

        public bool HasTarget => _attack.HasTarget;

        public TimedAttack(BulletConfig bulletConfig, Transform firePoint, float interval)
        {
            _attack = new Attack(bulletConfig, firePoint);
            _interval = interval;
            _currentTime = interval;
        }

        public void SetTarget(ITarget target)
        {
            _attack.SetTarget(target);
        }

        public void SetEnabled(bool enabled)
        {
            _enabled = enabled;
        }

        public void Tick(float deltaTime)
        {
            if (!_enabled)
                return;

            _currentTime -= deltaTime;
            if (_currentTime <= 0f)
            {
                _attack.Fire();
                _currentTime += _interval;
            }
        }

        public void Reset()
        {
            _enabled = false;
            _currentTime = _interval;
            _attack.Reset();
        }
    }
}
