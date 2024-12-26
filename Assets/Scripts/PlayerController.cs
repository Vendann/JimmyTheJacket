using System.Threading;
using UnityEngine;
using Cinemachine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController _controller;

    [Header("Jump and Gravity")]
    [SerializeField] private float _gravity = -9.81f; // Гравитация
    [SerializeField] private float _jumpHeight = 3f; // Высота прыжка
    private Vector3 _velocity;

    [Header("Run and Speed")]
    [SerializeField] private float _defaultSpeed = 12f; // Стандартная скорость движения
    [SerializeField] private float _sprintingSpeed = 18f; // Скорость во время спринта
    private float _moveForward;
    private float _moveRight;
    private bool _isSprinting = false;

    [Header("Ground Check")]
    [SerializeField] private Transform _groundCheck; // Точка проверки нахождения на земле
    [SerializeField] private float _groundCheckSphereRadius = 0.4f; // Радиус сферы для проверки
    [SerializeField] private LayerMask _groundMask; // Слой земли
    private bool _isGrounded = true;

    [Header("Crouch")]
    [SerializeField] private float _crouchedHeight = 1.5f; // Высота капсулы во время приседания
    [SerializeField] private float _standingHeight = 3f; // Высота капсулы в положении стоя
    [SerializeField] private float _crouchedSpeed = 4f; // Скорость при приседании
    private bool _isCrouched = false;

    private Vector3 _move;

    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    [SerializeField] private float _mouseSensitivity = 1000f;
    private float _xRotation = 0f;
    private float mouseX;
    private float mouseY;

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update() {
        InputManagement();
        Move();
        JumpAndGravity();
        CameraLook();
    }

    // Метод для считывания ввода
    private void InputManagement() {
        _moveRight = Input.GetAxis("Horizontal");
        _moveForward = Input.GetAxis("Vertical");

        mouseX = Input.GetAxis("Mouse X");
        mouseY = Input.GetAxis("Mouse Y");

        if (Input.GetButtonDown("Sprint")) _isSprinting = ToggleSprint(_isSprinting);
        if (Input.GetButtonDown("Crouch")) _isCrouched = ToggleCrouch(_isCrouched);
    }

    // Метод движения
    private void Move() {
        if (_moveForward == 0 && _moveRight == 0) return;

        _move = transform.right * _moveRight + transform.forward * _moveForward;

        if (_isCrouched) {
            _controller.Move(_move * _crouchedSpeed * Time.deltaTime);
            return;
        }
        else if (_isSprinting) {
            _controller.Move(_move * _sprintingSpeed * Time.deltaTime);
            return;
        }
        else _controller.Move(_move * _defaultSpeed * Time.deltaTime);
    }
        
    // Метод прыжка и гравитации, проверяет нахождение на земле
    private void JumpAndGravity() {
        _isGrounded = Physics.Raycast(_groundCheck.position, Vector3.down, _groundCheckSphereRadius ,_groundMask); // TODO: Заменить на Raycast
        
        if (_isGrounded && _velocity.y < 0) _velocity.y = -2f;
        if (Input.GetButtonDown("Jump") && _isGrounded && !_isCrouched) _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);

        _velocity.y += _gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }

    // Метод спринта
    private bool ToggleSprint(bool isSprinting) {
        return !isSprinting; // Возвращаем измененный режим
    }

    // Метод приседания, меняет размер капсулы и ее центр, а также положение камеры
    private bool ToggleCrouch(bool isCrouched) {
        if (_isCrouched) { // Тут игрок встает
            _controller.height = _standingHeight;
            _controller.center = Vector3.zero;
            _virtualCamera.m_Follow.transform.position += new Vector3(0f, _standingHeight / 2, 0f);
        }
        else { // Тут игрок приседает
            _controller.height = _crouchedHeight;
            _controller.center = new Vector3(0f, -_crouchedHeight / 2, 0f);
            _virtualCamera.m_Follow.transform.position -= new Vector3(0f, _standingHeight / 2, 0f);
        }
        return !isCrouched; // Возвращаем измененный режим
    }

    // Метод вращения камеры
    private void CameraLook() {
        if (mouseX == 0 && mouseY == 0) return;

        mouseX *= _mouseSensitivity * Time.deltaTime;
        mouseY *= _mouseSensitivity * Time.deltaTime;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        _virtualCamera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}