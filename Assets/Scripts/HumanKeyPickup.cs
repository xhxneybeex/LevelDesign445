using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HumanKeyPickup : MonoBehaviour
{
    [Header("Settings")]
    public string playerTag = "Player";
    
    [Header("Audio (Optional)")]
    public AudioClip pickupSound;
    
    private AudioSource audioSource;
    
    void Reset()
    {
        var collider = GetComponent<Collider>();
        collider.isTrigger = true;
        gameObject.tag = "Collectible";
    }
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) 
            return;
        
        if (other.TryGetComponent<PlayerInventoryHolder>(out var holder))
        {
            if (holder.Inventory.AddItem(ItemType.HumanKey))
            {
                if (audioSource != null && pickupSound != null)
                {
                    audioSource.PlayOneShot(pickupSound);
                }
                
                StatuePuzzleManager puzzleManager = FindFirstObjectByType<StatuePuzzleManager>();
                if (puzzleManager != null)
                {
                    puzzleManager.OnHumanKeyCollected();
                }
                
                Debug.Log("Picked up Human Key! You can now progress to the next scene.");
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Inventory full.");
            }
        }
    }
}
