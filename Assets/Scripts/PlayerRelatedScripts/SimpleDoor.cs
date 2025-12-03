using UnityEngine;
using System.Collections;

public class SimpleDoor : MonoBehaviour
{
    [Header("Rotation")]
    public float openAngle = 90f;     // how far to rotate on Y
    public float duration = 1f;       // how long the swing should take

    [Header("Blocking Collider (optional)")]
    public Collider blocker;          // assign if you want it, otherwise leave null

    bool isOpen = false;
    bool busy = false;

    Quaternion closedRot;
    Quaternion openRot;

    void Start()
    {
        // Cache the starting rotation as "closed"
        closedRot = transform.localRotation;
        openRot = closedRot * Quaternion.Euler(0f, openAngle, 0f);
    }

    public void Interact()
    {
        if (busy) return;

        Debug.Log("SimpleDoor.Interact called on " + name);
        StartCoroutine(SwingDoor());
    }

    IEnumerator SwingDoor()
    {
        busy = true;

        if (!isOpen && blocker != null)
            blocker.enabled = false;

        Quaternion start = transform.localRotation;
        Quaternion target = isOpen ? closedRot : openRot;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            // Smooth step easing for more natural swing
            t = Mathf.SmoothStep(0f, 1f, t);

            transform.localRotation = Quaternion.Slerp(start, target, t);
            yield return null;
        }

        transform.localRotation = target;
        isOpen = !isOpen;

        if (!isOpen && blocker != null)
            blocker.enabled = true;

        busy = false;
    }
}
