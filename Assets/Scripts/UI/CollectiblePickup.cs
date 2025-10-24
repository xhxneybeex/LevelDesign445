using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CollectiblePickup : MonoBehaviour
{
    public string playerTag = "Player";

    void Reset()
    {
        var c = GetComponent<Collider>();
        c.isTrigger = true;
        gameObject.tag = "Collectible"; // helpful default
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        if (other.TryGetComponent<PlayerInventoryHolder>(out var holder))
        {
            if (holder.Inventory.AddOne())
            {
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Inventory full.");
            }
        }
    }
}
