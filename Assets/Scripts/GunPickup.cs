using UnityEngine;

public class GunPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // find GunInHand object on the player and activate it
        Transform child = other.transform.Find("GunInHand");
        if (child != null)
        {
            child.gameObject.SetActive(true);

            // Get the GunController component and call Equip()
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

            // Destroy pickup object so it can't be picked up again
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("GunPickup: Could not find 'GunInHand' under player!");
        }
    }
}
