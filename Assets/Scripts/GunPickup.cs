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

        if (!other.TryGetComponent<PlayerInventoryHolder>(out var holder))
            return;

        InventorySimple inv = holder.Inventory;

        // must have a coin
        if (!inv.HasItem(ItemType.Coin))
        {
            Debug.Log("This debug should not be seen if u picked up the coin");
            return;
        }

        // remove ONE coin
        RemoveOneCoin(inv);

        // find GunInHand object on the player and activate it
        Transform child = other.transform.Find("GunInHand");
        if (child != null)
        {
            child.gameObject.SetActive(true);
            Debug.Log("GunPickup: Activated GuninHand");
        }
        else
        {
            Debug.LogWarning("GunPickup: Could not find 'GunInHand' under player!");
        }

        // destroy this world gun
        //Destroy(gameObject);
    }

    void RemoveOneCoin(InventorySimple inv)
    {
        for (int i = 0; i < inv.slots.Length; i++)
        {
            if (inv.slots[i] == ItemType.Coin)
            {
                inv.ClearSlot(i); // calls Notify()
                return;
            }
        }
    }
}
