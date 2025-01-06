using UnityEngine;

public class Sway : MonoBehaviour {
    [Header("Position")]
    [SerializeField] private float amount = 0.02f;
    [SerializeField] private float maxAmount = 0.06f;
    [SerializeField] private float smoothAmount = 6f;

    [Header("Rotation")]
    [SerializeField] private float rotationAmount = 4f;
    [SerializeField] private float maxRotationAmount = 5f;
    [SerializeField] private float smoothRotation = 12f;

    [Space]
    [SerializeField] private bool rotationX = true, rotationY = true, rotationZ = true;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private float InputX, InputY;

    [SerializeField] private PlayerController _playerController;

    private void Start() {
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
    }

    private void Update() {
        CalculateSway();
        MoveSway();
        TiltSway();
    }

    private void CalculateSway() {
        InputX = _playerController.MouseX;
        InputY = _playerController.MouseY;
    }

    private void MoveSway() {
        float moveX = Mathf.Clamp(InputX * amount, -maxAmount, maxAmount);
        float moveY = Mathf.Clamp(InputY * amount, -maxAmount, maxAmount);

        Vector3 finalPosition = new Vector3(moveX, moveY, 0);

        transform.localPosition = Vector3.Lerp(transform.localPosition, finalPosition + initialPosition, Time.deltaTime * smoothAmount);
    }

    private void TiltSway() {
        float tiltY = Mathf.Clamp(InputX * rotationAmount, -maxRotationAmount, maxRotationAmount);
        float tiltX = Mathf.Clamp(InputY * rotationAmount, -maxRotationAmount, maxRotationAmount);

        Quaternion finalRotation = Quaternion.Euler(new Vector3(rotationX ? -tiltX : 0f, rotationY ? tiltY : 0f, rotationZ ? tiltY : 0f));

        transform.localRotation = Quaternion.Slerp(transform.localRotation, finalRotation * initialRotation, smoothRotation * Time.deltaTime);
    }
}