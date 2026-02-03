namespace ShootEmUp
{
    public interface IDamageable
    {
        bool IsPlayer { get; }
        int Health { get; set; }
    }
}