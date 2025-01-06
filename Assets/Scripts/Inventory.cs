using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] public int _maxAmmoPistol;
    [SerializeField] public int _maxAmmoShotgun;
    [SerializeField] public int _maxAmmoRifle;
    [SerializeField] public int _maxGrenades;
    [SerializeField] public int _maxAidKits;

    public int AmmoPistol { get; set; }
    public int AmmoShotgun { get; set; }
    public int AmmoRifle { get; set; }
    public int Grenades { get; set; }
    public int AidKits { get; set; }

    private int _selectedGun = 0;
    private bool _canSelectGun = true;
    public bool CanSelectGun { set => _canSelectGun = value; }

    public Text gunText;
    public Text ammoPistol;
    public Text grenades;
    public Text aidKits;

    private void Awake() {
        AmmoPistol = _maxAmmoPistol;
        AmmoRifle = _maxAmmoRifle;
        AmmoShotgun = _maxAmmoShotgun;
        Grenades = _maxGrenades;
        AidKits = _maxAidKits;
    }

    private void Start() {
        SelectGun();
    }

    private void Update() {
        if (!_canSelectGun) return;

        int previousGun = _selectedGun;

        if (Input.GetAxis("Mouse ScrollWheel") < 0f) {
            if (_selectedGun >= transform.childCount - 1) _selectedGun = 0;
            else _selectedGun++;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f) {
            if (_selectedGun <= 0) _selectedGun = transform.childCount - 1;
            else _selectedGun--;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) _selectedGun = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2) _selectedGun = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha3) && transform.childCount >= 3) _selectedGun = 2;

        if (previousGun != _selectedGun) SelectGun();

        Interface();
    }

    private void SelectGun() {
        int i = 0;
        foreach (Transform gun in transform) {
            if (i == _selectedGun) gun.gameObject.SetActive(true);
            else gun.gameObject.SetActive(false);
            i++;
        }
    }

    private void Interface() {
        if (_selectedGun == 0) {
            gunText.text = "Пистолет";
            ammoPistol.text = "Патроны: " + transform.GetChild(_selectedGun).GetComponent<Gun>()._bulletsLeft.ToString() + " / " + AmmoPistol.ToString();
        }
        else if (_selectedGun == 1) {
            gunText.text = "Дробовик";
            ammoPistol.text = "Патроны: " + transform.GetChild(_selectedGun).GetComponent<Gun>()._bulletsLeft.ToString() + " / " + AmmoShotgun.ToString();
        }

        else {
            gunText.text = "Автомат";
            ammoPistol.text = "Патроны: " + transform.GetChild(_selectedGun).GetComponent<Gun>()._bulletsLeft.ToString() + " / " + AmmoRifle.ToString();
        }

        grenades.text = "Гранаты: " + Grenades.ToString();
        aidKits.text = "Аптечки: " + AidKits.ToString();
    }
}
