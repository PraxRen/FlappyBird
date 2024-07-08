using System;

public interface IDamageable
{
    event Action<IDamageable> Died;
    event Action<IDamageDealer> TookDamage;

    bool IsDied { get; }

    void TakeDamage(IDamageDealer damageDealer);
}