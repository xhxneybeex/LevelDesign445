using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TriggerTextDisplay : MonoBehaviour
{
    [Header("Text Settings")]
    [TextArea(3, 5)]
    public string displayText = "Enter your text here";
    public float displayDuration = 3f;
    public bool triggerOnce = true;

    [Header("UI References")]
    public Text uiText; // Drag your UI Text here

    private bool hasTriggered = false;

    private void Start()
    {
        // Make sure text is hidden at start
        if (uiText != null)
        {
            uiText.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the player
        if (other.CompareTag("Player"))
        {
            // If set to trigger once and already triggered, return
            if (triggerOnce && hasTriggered)
                return;

            ShowText();
            hasTriggered = true;
        }
    }

    private void ShowText()
    {
        if (uiText != null)
        {
            StopAllCoroutines();
            StartCoroutine(DisplayTextCoroutine());
        }
        else
        {
            Debug.LogWarning("UI Text reference is not set on " + gameObject.name);
        }
    }

    private IEnumerator DisplayTextCoroutine()
    {
        uiText.text = displayText;
        uiText.gameObject.SetActive(true);

        yield return new WaitForSeconds(displayDuration);

        uiText.gameObject.SetActive(false);
    }

    public void ResetTrigger()
    {
        hasTriggered = false;
    }
}