using UnityEngine;

namespace ShootEmUp
{
    public sealed class AttackComponent : MonoBehaviour
    {
        private BulletConfig _bulletConfig;
        private Transform _firePoint;
        private ITarget _target;

        public bool HasTarget => _target != null && _target.IsAlive;

        public void Init(BulletConfig bulletConfig, Transform firePoint)
        {
            _bulletConfig = bulletConfig;
            _firePoint = firePoint;
        }

        public void SetTarget(ITarget target)
        {
            _target = target;
        }

        public void Fire()
        {
            if (_bulletConfig == null || _firePoint == null)
                return;

            Vector2 position = _firePoint.position;
            Vector2 direction = _target != null && _target.IsAlive
                ? (_target.Position - position).normalized
                : (Vector2)(_firePoint.rotation * Vector3.up);

            FireRequestService.RequestFire(_bulletConfig, position, direction);
        }

        public void Reset()
        {
            _target = null;
        }
    }
}
