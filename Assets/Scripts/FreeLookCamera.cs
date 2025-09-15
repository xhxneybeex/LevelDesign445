using UnityEngine;

public class FreeLookCamera : MonoBehaviour
{
    [Header("Target & Distance")]
    public Transform target;           // The character to follow
    public float distance = 5f;        // Distance behind the target
    public float heightOffset = 2f;    // Vertical offset from target position

    [Header("Rotation Settings")]
    public float sensitivity = 5f;
    public float verticalAngleMin = -30f;
    public float verticalAngleMax = 60f;

    [Header("Smoothing")]
    public float rotationSmoothTime = 0.1f;
    private Vector3 currentRotation;
    private Vector3 rotationSmoothVelocity;

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

        // Calculate camera position
        Quaternion rotation = Quaternion.Euler(currentRotation.x, currentRotation.y, 0);
        Vector3 offset = rotation * new Vector3(0, 0, -distance);
        Vector3 desiredPosition = target.position + Vector3.up * heightOffset + offset;

        transform.position = desiredPosition;
        transform.LookAt(target.position + Vector3.up * heightOffset);
    }
}
