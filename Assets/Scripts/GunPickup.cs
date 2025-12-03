using UnityEngine;

[RequireComponent(typeof(Collider))]
public class GunPickup : MonoBehaviour
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

        // get inventory
        if (!other.TryGetComponent<PlayerInventoryHolder>(out var holder))
            return;

        var inv = holder.Inventory;

        // 1. require a coin
        if (!inv.HasItem(ItemType.Coin))
        {
            Debug.Log("Need a coin to pick up this weapon.");
            return;
        }

        // 2. spend ONE coin
        RemoveOneCoin(inv);

        // 3. give the weapon
        var p = other.GetComponent<Player>();
        if (p != null)
        {
            p.EnableWeapon();
        }

        // 4. destroy gun pickup
        Destroy(gameObject);
    }

    void RemoveOneCoin(InventorySimple inv)
    {
        for (int i = 0; i < inv.slots.Length; i++)
        {
            if (inv.slots[i] == ItemType.Coin)
            {
                inv.slots[i] = ItemType.None;
                inv.Notify();   // refresh UI
                return;
            }
        }
    }
}
