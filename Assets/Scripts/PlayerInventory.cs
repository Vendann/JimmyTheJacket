using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour {
    private int medkits;
    private int energyDrinks;
    private int grenades;
    private int ammo;

    public void AddItem(Item.ItemType itemType) {
        switch (itemType)
        {
            case Item.ItemType.Medkit:
                medkits++;
                Debug.Log("Подобрана аптечка. Всего аптечек: " + medkits);
                break;
            case Item.ItemType.EnergyDrink:
                energyDrinks++;
                Debug.Log("Подобран энергетик. Всего энергетиков: " + energyDrinks);
                break;
            case Item.ItemType.Grenade:
                grenades++;
                Debug.Log("Подобрана граната. Всего гранат: " + grenades);
                break;
            case Item.ItemType.Ammo:
                ammo++;
                Debug.Log("Подобраны патроны. Всего патронов: " + ammo);
                break;
        }
    }
}
