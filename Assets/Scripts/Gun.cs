using UnityEngine;
using System.Collections;

public enum AmmunitionType {
    Pistol, Shotgun, Rifle
}

public class Gun : MonoBehaviour
{
    [Header("Gun Characteristics")]
    [SerializeField] private float _damage;
    [SerializeField] private float _timeBetweenShooting;
    [SerializeField] private float _spread;
    [SerializeField] private float _spreadZoomed;
    [SerializeField] private float _fireRange;
    [SerializeField] private float _reloadTime;
    [SerializeField] private float _timeBetweenShots;
    [SerializeField] private int _magazineSize;
    [SerializeField] private int _bulletsPerShot;
    [SerializeField] private bool _allowButtonHoldToShoot;
    [SerializeField] private AmmunitionType _ammunitionType;

    public int _bulletsLeft;
    private int _bulletsShot;

    private bool _zooming;
    private bool _shooting;
    private bool _readyToShoot;
    private bool _reloading;

    private float _currentSpread;

    [Header("References")]
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Camera _gunCamera;
    [SerializeField] private LayerMask _layerMask = new LayerMask();
    [SerializeField] private ParticleSystem _muzzleFlash;
    [SerializeField] private GameObject _bulletHoleDecal;
    [SerializeField] private Inventory _inventory;
    [SerializeField] private GameObject shotSound;

    private RaycastHit _hit;

    [Header("Camera Shaker Parameters")]
    [SerializeField] private float _durationCamera;
    [SerializeField] private float _rotationAngleCamera;
    [SerializeField] private float _distanceCamera;
    [SerializeField] private Vector3 _rotationDirectionCamera;
    [SerializeField] private Vector3 _moveDirectionCamera;

    [Header("Gun Recoil Parameters")]
    [SerializeField] private float _durationGun;
    [SerializeField] private float _rotationAngleGun;
    [SerializeField] private float _distanceGun;
    [SerializeField] private Vector3 _rotationDirectionGun;
    [SerializeField] private Vector3 _moveDirectionGun;

    [Header("FOV Zoom Parameters")]
    [SerializeField] private float _defaultFOV = 90f;
    [SerializeField] private float _zoomedFOV = 60f;
    [SerializeField] private float _transitionSpeed = 0.5f;

    private IEnumerator _coroutineZoomFOV;

    [Header("Gun Offset Parameters")]
    [SerializeField] private Vector3 _normalPosition;
    [SerializeField] private Vector3 _zoomingPosition;
    [SerializeField] private Vector3 _reloadingPosition;
    [SerializeField] private Quaternion _normalRotation;
    [SerializeField] private Quaternion _reloadRotation;

    private IEnumerator _coroutineMoveGun;
    private IEnumerator _coroutineGunReloadRotation;

    private bool _isZooming;

    [Header("Settings Menu")]
    [SerializeField] private bool _enableAutoReload;

    public SoundManager SoundManager;

    private void Awake() {
        _bulletsLeft = _magazineSize;
        _readyToShoot = true;

        _mainCamera.fieldOfView = _defaultFOV;
        _gunCamera.fieldOfView = _defaultFOV;
        
        _currentSpread = _spread;
        _normalPosition = transform.localPosition;

        _normalRotation = transform.localRotation;
    }

    private void OnEnable() {
        transform.localPosition = _normalPosition;
        transform.localRotation = _normalRotation;
        Zoom(false);
    }

    private void Update() {
        InputManagement();
    }

    private void InputManagement() {
        if (_allowButtonHoldToShoot) _shooting = Input.GetButton("Fire1");
        else _shooting = Input.GetButtonDown("Fire1");

        if (!_reloading) {
            if (Input.GetButtonDown("Fire2") && !_isZooming) {
                Zoom(true);
                ZoomGun(true);
            }
            else if (Input.GetButtonDown("Fire2") && _isZooming) {
                Zoom(false);
                ZoomGun(false);
            }
        }
        
        if (Input.GetButtonDown("ReloadGun") && _bulletsLeft < _magazineSize && !_reloading) ReloadGun();

        if (_readyToShoot && _shooting && !_reloading && _bulletsLeft > 0) {
            _bulletsShot = _bulletsPerShot;
            Shoot();
            
            ShakeRotate(_durationCamera, _rotationAngleCamera, _rotationDirectionCamera, _mainCamera.transform);
            ShakeMove(_durationCamera, _distanceCamera, _moveDirectionCamera, _mainCamera.transform);

            if (!_isZooming) {
                ShakeRotate(_durationGun, _rotationAngleGun, _rotationDirectionGun, transform);
                ShakeMove(_durationGun, _distanceGun, _moveDirectionGun, transform);
            }

            else ShakeMove(_durationGun, _distanceGun, new Vector3(0f, 0f, _moveDirectionGun.z), transform);
        }
    }

    private void ReloadGun() {
        if (_ammunitionType == AmmunitionType.Pistol && _inventory.AmmoPistol <= 0) return;

        else if (_ammunitionType == AmmunitionType.Shotgun && _inventory.AmmoShotgun <= 0) return;

        else if (_ammunitionType == AmmunitionType.Rifle && _inventory.AmmoRifle <= 0) return;

        _inventory.CanSelectGun = false;
        Zoom(false);
        _reloading = true;

        if (_ammunitionType == AmmunitionType.Pistol) SoundManager.Instance.pistolReload.Play();

        else if (_ammunitionType == AmmunitionType.Shotgun) SoundManager.Instance.shotgunReload.Play();

        else if (_ammunitionType == AmmunitionType.Rifle) SoundManager.Instance.rifleReload.Play();

        ReloadRotateGun(_reloadTime);
        Invoke("ReloadFinished", _reloadTime);
    }

    private void ReloadFinished() {
        Zoom(false);

        if (_ammunitionType == AmmunitionType.Pistol && _inventory.AmmoPistol >= _magazineSize) _inventory.AmmoPistol -= _magazineSize;

        else if (_ammunitionType == AmmunitionType.Shotgun && _inventory.AmmoShotgun >= _magazineSize) _inventory.AmmoShotgun -= _magazineSize;

        else if (_ammunitionType == AmmunitionType.Rifle && _inventory.AmmoRifle >= _magazineSize) _inventory.AmmoRifle -= _magazineSize;

        _bulletsLeft = _magazineSize;
        _reloading = false;
        _inventory.CanSelectGun = true;
    }

    private void Shoot() {
        _readyToShoot = false;

        float x, y;

            x = Random.Range(-_currentSpread, _currentSpread);
            y = Random.Range(-_currentSpread, _currentSpread);

        Vector2 dir = new Vector2(x, y);
        dir.Normalize();

        Vector3 shotDirection = _mainCamera.transform.forward + new Vector3(x, y, 0);

        if (Physics.Raycast(_mainCamera.transform.position, shotDirection, out _hit, _fireRange, _layerMask)) {
            var damageable = _hit.transform.GetComponent<IDamageable>();
            if (damageable != null) damageable.ReceiveDamage(_damage, _hit.point);
        }

        ShootVisual();

        _bulletsLeft--;
        _bulletsShot--;

        GameObject sound = Instantiate(shotSound, transform.position, Quaternion.identity);
        Destroy(sound, 1f);

        if (!IsInvoking("ResetShot") && !_readyToShoot)
            Invoke("ResetShot", _timeBetweenShooting);

        if (_bulletsShot > 0 && _bulletsLeft > 0) Invoke("Shoot", _timeBetweenShots);

        if (_enableAutoReload && _bulletsLeft <= 0 && !_reloading) Invoke("ReloadGun", 0.5f);
    }

    private void ResetShot() {
        _readyToShoot = true;
    }

    private void ShootVisual() {
        _muzzleFlash.Play();
        //if (_hit.transform.gameObject.tag != "Enemy") {
            GameObject bulletHole = Instantiate(_bulletHoleDecal, _hit.point, Quaternion.LookRotation(_hit.normal));
            bulletHole.transform.position += bulletHole.transform.forward / 1000f;
            Destroy(bulletHole, 5f);
        //}
    }

    private void ShakeMove(float duration, float distance, Vector3 direction, Transform objectTransform) {
        StartCoroutine(ShakeMoveCor(duration, distance, direction, objectTransform));
    }

    private void ShakeRotate(float duration, float angleDeg, Vector3 direction, Transform objectTransform) {
        StartCoroutine(ShakeRotateCor(duration, angleDeg, direction, objectTransform));
    }

    private IEnumerator ShakeRotateCor(float duration, float angleDeg, Vector3 direction, Transform objectTransform) {
        float elapsed = 0f;
        Quaternion startRotation = objectTransform.localRotation;

        float halfDuration = duration / 2;
        direction = direction.normalized;
        while (elapsed < duration) {
            Vector3 currentDirection = direction;
            float t = elapsed < halfDuration ? elapsed / halfDuration : (duration - elapsed) / halfDuration;
            float currentAngle = Mathf.Lerp(0f, angleDeg, t);
            currentDirection *= Mathf.Tan(currentAngle * Mathf.Deg2Rad);
            Vector3 resDirection = (currentDirection + Vector3.forward).normalized;
            objectTransform.localRotation = Quaternion.FromToRotation(Vector3.forward, resDirection);

            elapsed += Time.deltaTime;
            yield return null;
        }
        objectTransform.localRotation = startRotation;
    }

    private IEnumerator ShakeMoveCor(float duration, float distance, Vector3 direction, Transform objectTransform) {
        float elapsed = 0f;
        Vector3 startPosition = objectTransform.localPosition;

        float halfDuration = duration / 2;
        direction = direction.normalized;
        while (elapsed < duration) {
            float t = elapsed < halfDuration ? elapsed / halfDuration : (duration - elapsed) / halfDuration;
            float currentDistance = Mathf.Lerp(0f, distance, t);
            Vector3 currentOffset = direction * currentDistance;
            objectTransform.localPosition = startPosition + currentOffset;

            elapsed += Time.deltaTime;
            yield return null;
        }
        objectTransform.localPosition = startPosition;
    }

    public void Zoom(bool isZooming) {
        if (_reloading) return;
        if (_coroutineZoomFOV != null) StopCoroutine(_coroutineZoomFOV);

        if (isZooming) {
            _coroutineZoomFOV = CoroutineZoomFOV(_zoomedFOV, _transitionSpeed, isZooming);
            _currentSpread = _spreadZoomed;
        }
        else {
            _coroutineZoomFOV = CoroutineZoomFOV(_defaultFOV, _transitionSpeed, isZooming);
            _currentSpread = _spread;
        }

        StartCoroutine(_coroutineZoomFOV);
    }

    public void ZoomGun(bool isZooming) {
        if (_reloading) return;
        if (_coroutineMoveGun != null) StopCoroutine(_coroutineMoveGun);

        if (isZooming) {
            _coroutineMoveGun = CoroutineMoveGun(_zoomingPosition, _transitionSpeed);
            _currentSpread = _spreadZoomed;
        }
        else {
            _coroutineMoveGun = CoroutineMoveGun(_normalPosition, _transitionSpeed);
            _currentSpread = _spread;
        }

        StartCoroutine(_coroutineMoveGun);
    }

    private IEnumerator CoroutineZoomFOV(float targetFOV, float duration, bool isZooming) {
        if (isZooming) _isZooming = isZooming;

        float startFOV = _mainCamera.fieldOfView;
        float step = Mathf.Abs(targetFOV - startFOV) / duration;

        while (!Mathf.Approximately(_mainCamera.fieldOfView, targetFOV)) {
            _mainCamera.fieldOfView = Mathf.MoveTowards(_mainCamera.fieldOfView, targetFOV, step * Time.deltaTime);
            _gunCamera.fieldOfView = _mainCamera.fieldOfView;
            yield return null;
        }

        _mainCamera.fieldOfView = targetFOV;
        _gunCamera.fieldOfView = targetFOV;

        if (!isZooming) _isZooming = isZooming;

        _coroutineZoomFOV = null;
    }

    private IEnumerator CoroutineMoveGun(Vector3 targetPosition, float duration) {

        Vector3 startPosition = transform.localPosition;
        float step = Vector3.Distance(startPosition, targetPosition) / duration;

        while (!Mathf.Approximately(Vector3.Distance(transform.localPosition, targetPosition), 0f)) {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, targetPosition, step * Time.deltaTime);
            yield return null;
        }

        transform.localPosition = targetPosition;
        _coroutineMoveGun = null;
    }

    private void ReloadRotateGun(float duration) {
        if (_coroutineGunReloadRotation != null) StopCoroutine(_coroutineGunReloadRotation);

        if (_reloading) {
            _coroutineGunReloadRotation = CoroutineReloadRotateGun(duration);
            StartCoroutine(_coroutineGunReloadRotation);
        }
    }

    private IEnumerator CoroutineReloadRotateGun(float duration) {
        Quaternion startRotation = transform.localRotation;
        float angleDistance = Quaternion.Angle(startRotation, _reloadRotation);
        float step = angleDistance / (duration - (duration - 0.2f));

        Vector3 startPosition = transform.localPosition;
        float step1 = Vector3.Distance(startPosition, _reloadingPosition) / (duration - (duration - 0.2f));

        while (!Mathf.Approximately(Quaternion.Angle(transform.localRotation, _reloadRotation), 0f)) {
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, _reloadRotation, step * Time.deltaTime);

            transform.localPosition = Vector3.MoveTowards(transform.localPosition, _reloadingPosition, step1 * Time.deltaTime);

            yield return null;
        }

        transform.localRotation = _reloadRotation;
        transform.localPosition = _reloadingPosition;

        yield return new WaitForSeconds(duration - 0.4f);

        float startAngleDistance = Quaternion.Angle(transform.localRotation, _normalRotation);
        while (!Mathf.Approximately(Quaternion.Angle(transform.localRotation, _normalRotation), 0f)) {
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, _normalRotation, step * Time.deltaTime);

            transform.localPosition = Vector3.MoveTowards(transform.localPosition, _normalPosition, step1 * Time.deltaTime);

            yield return null;
        }

        transform.localRotation = _normalRotation;

        transform.localPosition = _normalPosition;

        _coroutineGunReloadRotation = null;
    }
}