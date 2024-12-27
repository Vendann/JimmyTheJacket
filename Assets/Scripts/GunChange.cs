using UnityEngine;

public class GunChange : MonoBehaviour
{
    private int _selectedGun = 0;

    private void Start() {
        SelectGun();
    }

    private void Update() {
        int previousGun = _selectedGun;

        if (Input.GetAxis("Mouse ScrollWheel") > 0f) {
            if (_selectedGun >= transform.childCount - 1) _selectedGun = 0;
            else _selectedGun++;
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0f) {
            if (_selectedGun <= 0) _selectedGun = transform.childCount - 1;
            else _selectedGun--;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) _selectedGun = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2) _selectedGun = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha3) && transform.childCount >= 3) _selectedGun = 2;

        if (previousGun != _selectedGun) SelectGun();
    }

    private void SelectGun() {
        int i = 0;
        foreach (Transform gun in transform) {
            if (i == _selectedGun) gun.gameObject.SetActive(true);
            else gun.gameObject.SetActive(false);
            i++;
        }
    }
}
