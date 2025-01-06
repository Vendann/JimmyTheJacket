using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] private Transform _interactorSource;
    [SerializeField] private float _interactionDistance = 4f;

    private RaycastHit _hit;

    private void Update() {
        if (Input.GetButtonDown("Interact")) {
            if (Physics.Raycast(_interactorSource.position, _interactorSource.forward, out _hit, _interactionDistance)) {
                if (_hit.collider.gameObject.TryGetComponent(out Interactable interactObj)) {
                    SoundManager.Instance.itemPickUp.Play();
                    interactObj.AddItem();
                }
            }
        }
    }
}
