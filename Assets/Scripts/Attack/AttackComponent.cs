using UnityEngine;

namespace ShootEmUp
{
    public sealed class AttackComponent : MonoBehaviour
    {
        [SerializeField] private float _attackInterval = 1f;

        private BulletConfig _bulletConfig;
        private Transform _firePoint;
        private ITarget _target;

        public void Init(BulletConfig bulletConfig, Transform firePoint)
        {
            _bulletConfig = bulletConfig;
            _firePoint = firePoint;
        }

        private float _currentTime;
        private bool _isActive;

        private void FixedUpdate()
        {
            if (!_isActive)
                return;

            _currentTime -= Time.fixedDeltaTime;
            if (_currentTime <= 0f)
            {
                TryFire();
                _currentTime += _attackInterval;
            }
        }

        public void Reset()
        {
            _currentTime = _attackInterval;
        }

        public void SetTarget(ITarget target)
        {
            _target = target;
        }

        public void SetCanFire(bool canFire)
        {
            _isActive = canFire;
        }

        private void TryFire()
        {
            if (_target == null || !_target.IsAlive || _bulletConfig == null || _firePoint == null)
                return;

            Vector2 position = _firePoint.position;
            Vector2 direction = (_target.Position - position).normalized;
            FireRequestService.RequestFire(_bulletConfig, position, direction);
        }

    }
}
