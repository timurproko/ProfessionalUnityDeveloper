using System;
using UnityEngine;

namespace ShootEmUp
{
    public sealed class Attack
    {
        private readonly BulletConfig _bulletConfig;
        private readonly Transform _firePoint;
        private Transform _aimAt;
        private Func<bool> _isAlive;

        public Attack(BulletConfig bulletConfig, Transform firePoint)
        {
            _bulletConfig = bulletConfig;
            _firePoint = firePoint;
        }

        public bool HasTarget => _aimAt != null && (_isAlive?.Invoke() ?? false);

        public void SetTarget(Transform aimAt, Func<bool> isAlive)
        {
            _aimAt = aimAt;
            _isAlive = isAlive;
        }

        public void Fire()
        {
            if (_bulletConfig == null || _firePoint == null)
                return;

            Vector2 position = _firePoint.position;
            Vector2 direction = HasTarget
                ? ((Vector2)_aimAt.position - position).normalized
                : (Vector2)(_firePoint.rotation * Vector3.up);

            FireRequestService.RequestFire(_bulletConfig, position, direction);
        }

        public void Reset()
        {
            _aimAt = null;
            _isAlive = null;
        }
    }
}
