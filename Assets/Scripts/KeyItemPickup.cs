using UnityEngine;

[RequireComponent(typeof(Collider))]
public class KeyItemPickup : MonoBehaviour
{
    [Header("Key Configuration")]
    public ItemType keyType;
    
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
            if (holder.Inventory.AddItem(keyType))
            {
                if (audioSource != null && pickupSound != null)
                {
                    audioSource.PlayOneShot(pickupSound);
                }
                
                StatuePuzzleManager puzzleManager = FindFirstObjectByType<StatuePuzzleManager>();
                if (puzzleManager != null)
                {
                    puzzleManager.OnKeyCollected(keyType);
                }
                
                Debug.Log($"Picked up {keyType}!");
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("Inventory full.");
            }
        }
    }
}
