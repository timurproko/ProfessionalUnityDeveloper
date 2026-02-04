using System;

namespace ShootEmUp
{
    public static class FireRequestChannel
    {
        public static event Action<FireRequest> OnFireRequested;

        public static void Raise(FireRequest request)
        {
            OnFireRequested?.Invoke(request);
        }
    }
}
