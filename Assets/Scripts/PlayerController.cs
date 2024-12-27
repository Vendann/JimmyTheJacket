using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private CharacterController _controller;

    [Header("Jump and Gravity")]
    [SerializeField] private float _gravity = -9.81f; // ����������
    [SerializeField] private float _jumpHeight = 3f; // ������ ������
    private Vector3 _velocity;

    [Header("Run and Speed")]
    [SerializeField] private float _defaultSpeed = 12f; // ����������� �������� ��������
    [SerializeField] private float _sprintingSpeed = 18f; // �������� �� ����� �������
    private float _moveForward;
    private float _moveRight;
    private bool _isSprinting = false;

    [Header("Ground Check")]
    [SerializeField] private Transform _groundCheck; // ����� �������� ���������� �� �����
    [SerializeField] private float _groundCheckSphereRadius = 0.4f; // ������ ����� ��� ��������
    [SerializeField] private LayerMask _groundMask; // ���� �����
    private bool _isGrounded = true;

    [Header("Crouch")]
    [SerializeField] private float _crouchedHeight = 1.5f; // ������ ������� �� ����� ����������
    [SerializeField] private float _standingHeight = 3f; // ������ ������� � ��������� ����
    [SerializeField] private float _crouchedSpeed = 4f; // �������� ��� ����������
    private bool _isCrouched = false;

    private Vector3 _move;

    [Header("Camera")]
    [SerializeField] private float _mouseSensitivity = 1000f;
    [SerializeField] private Camera _camera;
    [SerializeField] private Transform _cameraTransform;
    private float _mouseX;
    private float _mouseY;
    private float _xRotation = 0f;

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update() {
        InputManagement();
        Move();
        JumpAndGravity();
        CameraRotation();
    }

    // ����� ��� ���������� �����
    private void InputManagement() {
        _moveRight = Input.GetAxis("Horizontal");
        _moveForward = Input.GetAxis("Vertical");

        _mouseX = Input.GetAxis("Mouse X");
        _mouseY = Input.GetAxis("Mouse Y");

        if (Input.GetButtonDown("Sprint")) _isSprinting = ToggleSprint(_isSprinting);
        if (Input.GetButtonDown("Crouch")) _isCrouched = ToggleCrouch(_isCrouched);
    }

    // ����� ��������
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
        
    // ����� ������ � ����������, ��������� ���������� �� �����
    private void JumpAndGravity() {
        _isGrounded = Physics.Raycast(_groundCheck.position, Vector3.down, _groundCheckSphereRadius ,_groundMask); // TODO: �������� �� Raycast
        
        if (_isGrounded && _velocity.y < 0) _velocity.y = -2f;
        if (Input.GetButtonDown("Jump") && _isGrounded && !_isCrouched) _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);

        _velocity.y += _gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }

    // ����� �������
    private bool ToggleSprint(bool isSprinting) {
        if (!_isGrounded) return isSprinting;
        if (_isCrouched) return false;

        if (isSprinting) _camera.fieldOfView = 90f;
        else _camera.fieldOfView = 100f;
        return !isSprinting; // ���������� ���������� �����
    }

    // ����� ����������, ������ ������ ������� � �� �����, � ����� ��������� ������
    private bool ToggleCrouch(bool isCrouched) {
        if (!_isGrounded) return isCrouched;

        if (_isCrouched) { // ��� ����� ������
            _controller.height = _standingHeight;
            _controller.center = Vector3.zero;
            _cameraTransform.position += new Vector3(0f, _standingHeight / 2, 0f);
        }
        else { // ��� ����� ���������
            ToggleSprint(true);
            _controller.height = _crouchedHeight;
            _controller.center = new Vector3(0f, -_crouchedHeight / 2, 0f);
            _cameraTransform.position -= new Vector3(0f, _standingHeight / 2, 0f);
        }
        return !isCrouched; // ���������� ���������� �����
    }

    // ����� �������� ������
    private void CameraRotation() {
        if (_mouseX == 0 && _mouseY == 0) return;

        _mouseX *= _mouseSensitivity * Time.deltaTime;
        _mouseY *= _mouseSensitivity * Time.deltaTime;

        _xRotation -= _mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        _cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * _mouseX);
    }
}