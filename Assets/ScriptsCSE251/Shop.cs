using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Shop : MonoBehaviour
{
    public string playerTag = "Player";

    void Reset()
    {
        var c = GetComponent<Collider>();
        c.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        // Get the inventory
        if (other.TryGetComponent<PlayerInventoryHolder>(out var holder))
        {
            var inv = holder.Inventory;

            // Check if player has a coin
            if (inv.HasItem(ItemType.Coin))
            {
                // Remove ONE coin
                RemoveOneCoin(inv);

                // Grant the weapon
                var p = other.GetComponent<Player>();
                if (p != null) p.EnableWeapon();

                // Destroy gun pickup object
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Need a coin to pick up weapon.");
            }
        }
    }

    void RemoveOneCoin(InventorySimple inv)
    {
        for (int i = 0; i < inv.slots.Length; i++)
        {
            if (inv.slots[i] == ItemType.Coin)
            {
                inv.slots[i] = ItemType.None;
                inv.Notify();   // tell UI to refresh
                return;
            }
        }
    }

}
