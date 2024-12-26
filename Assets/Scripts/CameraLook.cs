using UnityEngine;

public class CameraLook : MonoBehaviour
{
    [SerializeField] private float _mouseSensitivity = 1000f;
    [SerializeField] private Transform playerBody;

    private float mouseX;
    private float mouseY;

    private float _xRotation = 0f;

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update() {
        InputManagement();
        CameraRotation();
    }

    private void InputManagement() {
        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");
    }

    private void CameraRotation() {
        if (mouseX == 0 && mouseY == 0) return;

        mouseX *= _mouseSensitivity * Time.deltaTime;
        mouseY *= _mouseSensitivity * Time.deltaTime;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
