using UnityEngine;
using System.Collections;

public class SimpleDoor : MonoBehaviour
{
    [Header("Rotation")]
    public float openAngle = 90f;     // how far to rotate on Y
    public float speed = 3f;          // how fast to rotate

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

        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        busy = true;

        if (!isOpen && blocker != null)
        {
            // about to OPEN → disable blocking if you use it
            blocker.enabled = false;
        }

        Quaternion start = transform.localRotation;
        Quaternion target = isOpen ? closedRot : openRot;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * speed;
            transform.localRotation = Quaternion.Slerp(start, target, t);
            yield return null;
        }

        transform.localRotation = target;
        isOpen = !isOpen;

        // just closed → re-enable blocker
        if (!isOpen && blocker != null)
            blocker.enabled = true;

        busy = false;
    }
}
