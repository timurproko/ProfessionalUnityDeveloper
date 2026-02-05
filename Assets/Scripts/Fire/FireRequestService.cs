using UnityEngine;

namespace ShootEmUp
{
    public static class FireRequestService
    {
        public static void RequestFire(BulletConfig config, Vector2 fromPosition, Vector2 direction)
        {
            if (config == null)
                return;

            FireRequest request = FireRequest.Configure(config, fromPosition, direction);
            FireRequestChannel.Raise(request);
        }
    }
}
