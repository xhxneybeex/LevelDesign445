using UnityEngine;

public class GunPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayerInventoryHolder inventoryHolder = other.GetComponent<PlayerInventoryHolder>();
        if (inventoryHolder == null)
        {
            Debug.LogWarning("GunPickup: Player doesn't have PlayerInventoryHolder component!");
            return;
        }

        if (!inventoryHolder.Inventory.HasItem(ItemType.Coin))
        {
            Debug.Log("GunPickup: You need to collect the coin first!");
            return;
        }

        Transform child = other.transform.Find("GunInHand");
        if (child != null)
        {
            child.gameObject.SetActive(true);

            GunController gunController = child.GetComponent<GunController>();
            if (gunController != null)
            {
                gunController.Equip();
                Debug.Log("GunPickup: Activated and equipped GunInHand");
            }
            else
            {
                Debug.LogWarning("GunPickup: GunInHand has no GunController component!");
            }

            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("GunPickup: Could not find 'GunInHand' under player!");
        }
    }
}
