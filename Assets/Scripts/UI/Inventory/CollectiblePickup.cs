using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CollectiblePickup : MonoBehaviour
{
    public string playerTag = "Player";

    // what this pickup actually is
    public ItemType itemType = ItemType.Coin;

    void Reset()
    {
        var c = GetComponent<Collider>();
        c.isTrigger = true;
        gameObject.tag = "Collectible";
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (other.TryGetComponent<PlayerInventoryHolder>(out var holder))
        {
            if (holder.Inventory.AddItem(itemType))
            {
                Destroy(gameObject);   // picked up
            }
            else
            {
                Debug.Log("Inventory full.");
            }
        }
    }
}
