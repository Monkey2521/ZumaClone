public interface IDamageable
{
    public int MaxHP { get; }
    public int HP { get; }

    public void TakeDamage(int damage);

    public void Die();
}
