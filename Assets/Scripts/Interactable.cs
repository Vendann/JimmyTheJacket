using UnityEngine;

public class Interactable : MonoBehaviour
{
    [SerializeField] private ItemType _itemType;
    [SerializeField] private Links _links;
    private Inventory _inventory;

    private void Start() {
        _inventory = _links._inventory;
    }

    public void AddItem() {
        Debug.Log("Add");
        switch (_itemType) {
            
            case ItemType.AidKit:
                _inventory.AidKits = _inventory._maxAidKits;
                break;
            
            case ItemType.AmmoPistol:
                _inventory.AmmoPistol = _inventory._maxAmmoPistol;
                break;

            case ItemType.AmmoShotgun:
                _inventory.AmmoShotgun = _inventory._maxAmmoShotgun;
                break;

            case ItemType.AmmoRifle:
                _inventory.AmmoRifle = _inventory._maxAmmoRifle;
                break;
            
            case ItemType.Grenade:
                _inventory.Grenades = _inventory._maxGrenades;
                break;
        }
    }
}

public enum ItemType {
    AidKit, AmmoPistol, AmmoShotgun, AmmoRifle, Grenade
}
