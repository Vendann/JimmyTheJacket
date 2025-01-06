using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private float _grenadeDamage = 90f;
    [SerializeField] private float _delayBeforeExplosion = 4f;
    [SerializeField] private float _explosionRadius = 5f;
    
    private float _countdown;
    private bool _hasExploded = false;

    [SerializeField] private GameObject _explosionParticle;
    public AudioSource explosion;

    private void Start() {
        _countdown = _delayBeforeExplosion;
    }

    private void Update() {
        _countdown -= Time.deltaTime;
        if (_countdown <= 0 && !_hasExploded) {
            Explode();
            _hasExploded = true;
        }
    }

    private void Explode() {
        var explosionEffect = Instantiate(_explosionParticle, transform.position, transform.rotation);
        Destroy(explosionEffect, 0.5f);
        
        Collider[] colliders = Physics.OverlapSphere(transform.position, _explosionRadius);
        

        foreach (Collider collider in colliders) {
            var damageable = collider.transform.GetComponent<IDamageable>();
            if (damageable != null) damageable.ReceiveDamage(_grenadeDamage, transform.position);
        }

        SoundManager.Instance.explosion = explosion;
        SoundManager.Instance.explosion.Play();

        Destroy(gameObject);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(this.transform.position, _explosionRadius);
    }
}
