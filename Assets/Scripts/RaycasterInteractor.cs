using UnityEngine;

public class RaycastInteractor : MonoBehaviour
{
    [Header("References")]
    public Camera cam;                 // Main Camera (with Cinemachine Brain)
    public Transform player;           // Player root for reach checks (defaults to this.transform)

    [Header("Interact Filter")]
    public LayerMask mask = ~0;        // Include world layers. Exclude Player & Ignore Raycast in the Inspector.
    public string interactableTag = "Interactable";
    public float reach = 3.5f;         // Max distance from PLAYER to hit point
    public float aimMaxDistance = 12f; // Max ray length from CAMERA

    [Header("Cast Shape")]
    public bool useSphereCast = true;  // Friendlier for 3rd person
    public float sphereRadius = 0.3f;
    public QueryTriggerInteraction triggerMode = QueryTriggerInteraction.Collide; // Include trigger colliders

    [Header("Input")]
    public KeyCode interactKey = KeyCode.E;   // If your project is "Input System Only", set Active Input Handling = Both.

    [Header("UI (optional)")]
    public GameObject handIcon;        // Screen-space Image in a Canvas (Screen Space - Overlay)

    [Header("Debug")]
    public bool debugLogs = false;

    SimpleDoor currentDoor;

    void Awake()
    {
        if (!cam) cam = Camera.main;
        if (!player) player = transform;
        if (handIcon) handIcon.SetActive(false);
    }

    void Update()
    {
        currentDoor = null;

        // Ray from the SCREEN CENTER (matches Cinemachine view even if offset)
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        RaycastHit hit;
        bool hitSomething = useSphereCast
            ? Physics.SphereCast(ray, sphereRadius, out hit, aimMaxDistance, mask, triggerMode)
            : Physics.Raycast(ray, out hit, aimMaxDistance, mask, triggerMode);

        bool showHand = false;

        if (hitSomething)
        {
            float dist = Vector3.Distance(player.position, hit.point);

            // Accept tag on collider OR its parent
            Transform t = hit.collider.transform;
            bool tagged = t.CompareTag(interactableTag) || (t.parent && t.parent.CompareTag(interactableTag));

            if (debugLogs) Debug.Log($"[Interactor] Hit {t.name}  tagged:{tagged}  dist:{dist:0.00}");

            if (tagged && dist <= reach)
            {
                // Find a SimpleDoor on this object or any parent (works if collider is on a child, script on the hinge)
                currentDoor = hit.collider.GetComponentInParent<SimpleDoor>();
                showHand = currentDoor != null;
                if (debugLogs && currentDoor == null) Debug.Log("[Interactor] Interactable tagged but no SimpleDoor found in parents");
            }
        }

        // Toggle hand icon
        if (handIcon) handIcon.SetActive(showHand);

        // Interact
        if (currentDoor != null && Input.GetKeyDown(interactKey))
        {
            if (debugLogs) Debug.Log($"[Interactor] Interact → {currentDoor.name}");
            currentDoor.Interact();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (!cam) return;
        Ray r = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Gizmos.color = Color.cyan;
        if (useSphereCast)
            Gizmos.DrawWireSphere(r.origin + r.direction * Mathf.Min(aimMaxDistance, 2f), sphereRadius);
        else
            Gizmos.DrawLine(r.origin, r.origin + r.direction * Mathf.Min(aimMaxDistance, 2f));
    }
}
