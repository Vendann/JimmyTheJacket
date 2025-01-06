using UnityEngine;

public class GrenadeThrower : MonoBehaviour
{
    [SerializeField] private float _throwingForce = 40f;
    [SerializeField] private GameObject _grenadePrefab;
    [SerializeField] private Inventory _invenrory;

    private void Update() {
        InputManagement();
    }

    private void InputManagement() {
        if (Input.GetButtonDown("Grenade") && _invenrory.Grenades > 0) {
            ThrowGrenade();
        }
    }

    private void ThrowGrenade() {
        SoundManager.Instance.grenadeReady.Play();
        GameObject grenade = Instantiate(_grenadePrefab, transform.position, transform.rotation);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * _throwingForce, ForceMode.VelocityChange);
        _invenrory.Grenades--;
    }
}
