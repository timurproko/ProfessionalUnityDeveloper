using System;
using UnityEngine;

namespace ShootEmUp
{
    public static class FireRequestChannel
    {
        public static event Action<FireRequest> OnFireRequested;

        public static void Raise(IBulletConfigProvider requester, Vector2 position, Vector2 direction)
        {
            var request = new FireRequest(requester, position, direction);
            OnFireRequested?.Invoke(request);
        }
    }
}
