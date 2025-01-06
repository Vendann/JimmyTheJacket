using UnityEngine;
using UnityEngine.SceneManagement;

public class LivingObjectHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float _maxHealth;
    public float MaxHealth => _maxHealth;

    private float _health;
    public float Health => _health;
    public bool isPlayer;

    private void Awake() {
        _health = _maxHealth;
    }

    public void ReceiveDamage(float damageAmount, Vector3 hitPosition) {
        _health -= damageAmount;

        if (!isPlayer) {
            // SoundManager.Instance.damageReaction = GetComponent<Enemy>().damageReaction;
            // SoundManager.Instance.damageReaction.Play();
        }

        if (_health <= 0) {
            if (isPlayer) SceneManager.LoadScene(1);
            Destroy(gameObject);
        }
    }

    public void ReceiveHeal(float healAmount, Vector3 hitPosition) {
        if (_health < _maxHealth) _health += healAmount;
        if (_health > _maxHealth) _health = _maxHealth;
    }

    public void LoadScene() {
        SceneManager.LoadScene(1);
    }
}
