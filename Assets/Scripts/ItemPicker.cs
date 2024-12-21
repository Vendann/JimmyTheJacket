using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {
    public enum ItemType { Medkit, EnergyDrink, Grenade, Ammo }
    public ItemType itemType;

    // Вызывается при соприкосновении с игроком
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player"))
        {
            CollectItem(other.gameObject);
        }
    }

    // Собираем предмет и добавляем в инвентарь
    private void CollectItem(GameObject player) {
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            inventory.AddItem(itemType);
            Destroy(gameObject); // Уничтожаем объект после подбора
        }
    }
}
