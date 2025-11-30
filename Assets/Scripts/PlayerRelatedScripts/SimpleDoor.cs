using UnityEngine;
using System.Collections;

public class SimpleDoor : MonoBehaviour
{
    [Header("Rotation")]
    public float openAngle = 90f;     // degrees
    public float speed = 180f;        // deg/sec

    [Header("Blocking Collider")]
    public Collider blocker;          // assign the Door's BoxCollider here

    bool open, busy;

    public void Interact()
    {
        if (busy) return;
        open = !open;
        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        busy = true;

        // Opening → stop blocking so we never push the player
        if (open && blocker) blocker.enabled = false;

        float targetY = transform.localEulerAngles.y + (open ? openAngle : -openAngle);
        while (Mathf.Abs(Mathf.DeltaAngle(transform.localEulerAngles.y, targetY)) > 0.5f)
        {
            float y = Mathf.MoveTowardsAngle(transform.localEulerAngles.y, targetY, speed * Time.deltaTime);
            var e = transform.localEulerAngles; e.y = y; transform.localEulerAngles = e;
            yield return null;
        }

        // Closed → block again (only if space is clear)
        if (!open && blocker)
        {
            // optional safety: don’t re-enable if the player is inside the doorway
            // if (!Physics.CheckBox(blocker.bounds.center, blocker.bounds.extents * 0.95f, blocker.transform.rotation, LayerMask.GetMask("Player")))
            blocker.enabled = true;
        }

        busy = false;
    }
}
