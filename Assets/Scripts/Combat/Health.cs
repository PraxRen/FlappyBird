using System;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private float _maxValue;

    public event Action<IDamageable> Died;
    public event Action<IDamageDealer> TookDamage;
    public event Action ValueChanged;

    public bool IsDied { get; private set; }
    public float Value { get; private set; }
    public float MaxValue => _maxValue;

    private void Start()
    {
        UpdateValue(_maxValue);
    }

    public void TakeDamage(IDamageDealer damageDealer)
    {
        if (damageDealer == null)
            throw new ArgumentNullException(nameof(damageDealer));

        if (damageDealer.Damage < 0)
            throw new ArgumentOutOfRangeException(nameof(damageDealer.Damage));

        if (IsDied)
            return;

        UpdateValue(Value - damageDealer.Damage);
        TookDamage?.Invoke(damageDealer);

        if (Value == 0)
            Die();
    }

    private void UpdateValue(float value)
    {
        Value = Mathf.Clamp(value, 0, _maxValue);
        ValueChanged?.Invoke();
    }

    private void Die()
    {
        if (IsDied)
            return;

        IsDied = true;
        Died?.Invoke(this);
    }

    public void Reset()
    {
        IsDied = false;
        UpdateValue(_maxValue);
    }
}