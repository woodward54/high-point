using UnityEngine;

public interface IDamageable
{
    public Transform Transform { get; }
    public float CurrentHealth { get; }
    public float MaxHealth { get; }
    public void TakeDamage(float damage, Unit attackingUnit, float delay);
}