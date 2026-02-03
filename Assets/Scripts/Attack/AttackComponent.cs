using System;
using UnityEngine;

namespace ShootEmUp
{
    public sealed class AttackComponent : MonoBehaviour
    {
        [SerializeField] private float _interval = 1f;

        public event Action OnFireRequested;
        
        private float currentTime;
        private bool isActive;

        public void SetActive(bool active)
        {
            isActive = active;
        }

        public void Reset()
        {
            currentTime = _interval;
        }

        private void FixedUpdate()
        {
            if (!isActive)
                return;

            currentTime -= Time.fixedDeltaTime;
            if (currentTime <= 0f)
            {
                OnFireRequested?.Invoke();
                currentTime += _interval;
            }
        }
    }
}
