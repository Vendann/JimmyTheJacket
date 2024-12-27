using UnityEngine;

public interface IDamageable
{
    float Health { get; }
    float MaxHealth { get; }
    void ReceiveDamage(float damageAmount, Vector3 hitPosition);
    void ReceiveHeal(float healAmount, Vector3 hitPosition);
}
