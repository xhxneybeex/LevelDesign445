using UnityEngine;

public class FreeLookCamera : MonoBehaviour
{
    [Header("Target & Distance")]
    public Transform target;
    public float distance = 5f;
    public float heightOffset = 2f;

    [Header("Rotation Settings")]
    public float sensitivity = 5f;
    public float verticalAngleMin = -30f;
    public float verticalAngleMax = 60f;

    [Header("Smoothing")]
    public float rotationSmoothTime = 0.1f;
    private Vector3 currentRotation;
    private Vector3 rotationSmoothVelocity;

    [Header("Collision")]
    public float collisionRadius = 0.3f;
    public LayerMask collisionLayers;

    private float yaw;
    private float pitch;

    void Start()
    {
        if (!target)
        {
            Debug.LogWarning("FreeLookCamera: No target assigned.");
            enabled = false;
            return;
        }

        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void LateUpdate()
    {
        // Mouse input
        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;
        pitch = Mathf.Clamp(pitch, verticalAngleMin, verticalAngleMax);

        // Smooth rotation
        Vector3 targetRotation = new Vector3(pitch, yaw);
        currentRotation = Vector3.SmoothDamp(currentRotation, targetRotation, ref rotationSmoothVelocity, rotationSmoothTime);

        // Calculate desired position
        Quaternion rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0);
        Vector3 offset = rotation * new Vector3(0, 0, -distance);
        Vector3 collisionOrigin = target.position + Vector3.up * heightOffset;
        Vector3 desiredPosition = collisionOrigin + offset;

        // Collision check
        Vector3 direction = (desiredPosition - collisionOrigin).normalized;
        float maxDistance = distance;

        RaycastHit hit;
        if (Physics.SphereCast(collisionOrigin, collisionRadius, direction, out hit, maxDistance, collisionLayers))
        {
            float adjustedDistance = hit.distance - collisionRadius;
            adjustedDistance = Mathf.Clamp(adjustedDistance, 0.5f, distance);
            desiredPosition = collisionOrigin + direction * adjustedDistance;
        }

        // Apply position and look
        transform.position = desiredPosition;
        transform.LookAt(collisionOrigin);
    }
}
