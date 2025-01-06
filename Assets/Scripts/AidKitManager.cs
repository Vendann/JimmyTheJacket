using UnityEngine;

public class AidKitManager : MonoBehaviour
{
    [SerializeField] private Inventory _invenrory;
    [SerializeField] private LivingObjectHealth _health;
    [SerializeField] private float _healAmount;

    private void Update() {
        InputManagement();
    }

    private void InputManagement() {
        if (Input.GetButtonDown("Heal") && _invenrory.AidKits > 0) {
            _health.ReceiveHeal(_healAmount, transform.position);
            _invenrory.AidKits--;
        }
    }
}
