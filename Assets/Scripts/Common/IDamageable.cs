using System;

namespace ShootEmUp
{
    public interface IDamageable
    {
        event Action<IDamageable, int> OnHealthChanged;
        event Action<IDamageable> OnHealthEmpty;
        
        bool IsPlayer { get; }
        int Health { get; set; }
    }
}