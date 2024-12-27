using UnityEngine;
using EZCameraShake;

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
    [SerializeField] private Camera _cameraTransform;
    [SerializeField] private LayerMask _layerMask = new LayerMask();
    // TODO: Сделать вспышки при выстреле и эффекты попадания
    [SerializeField] private ParticleSystem _muzzleFlash;
    [SerializeField] private GameObject _bulletHoleDecal;

    private RaycastHit _hit;

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

        Vector3 shotDirection = _cameraTransform.transform.position + new Vector3(dir.x, dir.y, 0);

        if (Physics.Raycast(_cameraTransform.transform.position, _cameraTransform.transform.forward, out _hit, _fireRange, _layerMask)) {
            var damageable = _hit.transform.GetComponent<IDamageable>();
            if (damageable != null) damageable.ReceiveDamage(_damage, _hit.point);
        }

        CameraShaker.Instance.ShakeOnce(4f, 4f, 0f, 0.2f);

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
}
