using UnityEngine;
using EZCameraShake;
using System.Collections;

public class Gun : MonoBehaviour
{
    [Header("Gun Characteristics")]
    [SerializeField] private float _damage;
    [SerializeField] private float _timeBetweenShooting;
    [SerializeField] private float _spread;
    [SerializeField] private float _fireRange;
    [SerializeField] private float _reloadTime;
    [SerializeField] private float _timeBetweenShots;
    [SerializeField] private int _magazineSize;
    [SerializeField] private int _bulletsPerShot;
    [SerializeField] private bool _allowButtonHold;

    private int _bulletsLeft;
    private int _bulletsShot;

    private bool _shooting;
    private bool _readyToShoot;
    private bool _reloading;

    [Header("References")]
    [SerializeField] private Camera _camera;
    [SerializeField] private LayerMask _layerMask = new LayerMask();
    // TODO: Сделать вспышки при выстреле и эффекты попадания
    [SerializeField] private ParticleSystem _muzzleFlash;
    [SerializeField] private GameObject _bulletHoleDecal;

    private RaycastHit _hit;

    [Header("Camera Shaker Parameters")]
    [SerializeField] private float _duration;
    [SerializeField] private float _rotationAngle;
    [SerializeField] private Vector2 _rotationDirection;

    private void Awake() {
        _bulletsLeft = _magazineSize;
        _readyToShoot = true;
    }

    private void Update() {
        InputManagement();
    }

    private void InputManagement() {
        if (_allowButtonHold) _shooting = Input.GetButton("Fire1");
        else _shooting = Input.GetButtonDown("Fire1");

        if (Input.GetButtonDown("ReloadGun") && _bulletsLeft < _magazineSize && !_reloading) ReloadGun();

        if (_readyToShoot && _shooting && !_reloading && _bulletsLeft > 0) {
            _bulletsShot = _bulletsPerShot;
            Shoot();
            ShakeRotateCamera(_duration, _rotationAngle, _rotationDirection);
        }
    }

    private void ReloadGun() {
        _reloading = true;
        Invoke("ReloadFinished", _reloadTime);
    }

    private void ReloadFinished() {
        _bulletsLeft = _magazineSize;
        _reloading = false;
    }

    private void Shoot() {
        _readyToShoot = false;

        float x = Random.Range(-_spread, _spread);
        float y = Random.Range(-_spread, _spread);

        Vector2 dir = new Vector2(x, y);
        dir.Normalize();

        Vector3 shotDirection = _camera.transform.forward + new Vector3(x, y, 0);

        if (Physics.Raycast(_camera.transform.position, shotDirection, out _hit, _fireRange, _layerMask)) {
            var damageable = _hit.transform.GetComponent<IDamageable>();
            if (damageable != null) damageable.ReceiveDamage(_damage, _hit.point);
        }

        // CameraShaker.Instance.ShakeOnce(_magnitude, _roughness, _fadeInTime, _fadeOutTime, Vector3.zero, new Vector3(10f, 0f, 0f));

        ShootVisual();

        _bulletsLeft--;
        _bulletsShot--;

        if (!IsInvoking("ResetShot") && !_readyToShoot)
            Invoke("ResetShot", _timeBetweenShooting);

        if (_bulletsShot > 0 && _bulletsLeft > 0) Invoke("Shoot", _timeBetweenShots);
    }

    private void ResetShot() {
        _readyToShoot = true;
    }

    private void ShootVisual() {
        _muzzleFlash.Play();
        GameObject bulletHole = Instantiate(_bulletHoleDecal, _hit.point, Quaternion.LookRotation(_hit.normal));
        bulletHole.transform.position += bulletHole.transform.forward / 1000f;
        Destroy(bulletHole, 5f);
    }


    public void ShakeRotateCamera(float duration, float angleDeg, Vector2 direction) {
        StartCoroutine(ShakeRotateCor(duration, angleDeg, direction));
    }

    private IEnumerator ShakeRotateCor(float duration, float angleDeg, Vector2 direction) {
        float elapsed = 0f;
        Quaternion startRotation = _camera.transform.localRotation;

        float halfDuration = duration / 2;
        direction = direction.normalized;
        while (elapsed < duration) {
            Vector2 currentDirection = direction;
            float t = elapsed < halfDuration ? elapsed / halfDuration : (duration - elapsed) / halfDuration;
            float currentAngle = Mathf.Lerp(0f, angleDeg, t);
            currentDirection *= Mathf.Tan(currentAngle * Mathf.Deg2Rad);
            Vector3 resDirection = ((Vector3)currentDirection + Vector3.forward).normalized;
            _camera.transform.localRotation = Quaternion.FromToRotation(Vector3.forward, resDirection);

            elapsed += Time.deltaTime;
            yield return null;
        }
        _camera.transform.localRotation = startRotation;
    }
}
