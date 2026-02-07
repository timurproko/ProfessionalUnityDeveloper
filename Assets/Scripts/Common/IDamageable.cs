namespace ShootEmUp
{
    public interface IDamageable
    {
        int Health { get; set; }
        bool IsPlayer { get; }
    }
}
