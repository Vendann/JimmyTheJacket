using UnityEngine;

public class LivingObjectHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float _maxHealth;
    public float MaxHealth => _maxHealth;

    private float _health;
    public float Health => _health;

    private void Awake() {
        _health = _maxHealth;
    }

    public void ReceiveDamage(float damageAmount, Vector3 hitPosition) {
        _health -= damageAmount;
        if (_health <= 0) Destroy(gameObject);
    }

    public void ReceiveHeal(float healAmount, Vector3 hitPosition) {
        if (_health < _maxHealth) _health += healAmount;
    }
}
