using UnityEngine;

namespace ShootEmUp
{
    public interface ITarget
    {
        Vector2 Position { get; }
        bool IsAlive { get; }
    }
}
