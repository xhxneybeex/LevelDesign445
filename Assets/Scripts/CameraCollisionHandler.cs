using UnityEngine;

public class CameraCollisionHandler : MonoBehaviour
{
    public Transform pivot; // Assign the Camera Holder (parent)
    public float maxDistance = 5f;
    public float minDistance = 0.5f;
    public float collisionRadius = 0.3f;
    public LayerMask collisionLayers;

    private Vector3 currentVelocity;

    void LateUpdate()
    {
        if (!pivot) return;

        Vector3 origin = pivot.position;
        Vector3 desiredPos = origin - pivot.forward * maxDistance;
        Vector3 direction = (desiredPos - origin).normalized;

        RaycastHit hit;
        float targetDistance = maxDistance;

        if (Physics.SphereCast(origin, collisionRadius, direction, out hit, maxDistance, collisionLayers))
        {
            targetDistance = Mathf.Clamp(hit.distance - collisionRadius, minDistance, maxDistance);
        }

        Vector3 finalPos = origin - pivot.forward * targetDistance;
        transform.position = Vector3.SmoothDamp(transform.position, finalPos, ref currentVelocity, 0.05f);
    }
}
