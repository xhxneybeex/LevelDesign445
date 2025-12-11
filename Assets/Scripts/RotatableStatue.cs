using UnityEngine;

public class RotatableStatue : MonoBehaviour
{
    [Header("Statue Configuration")]
    public ItemType requiredKey;
    public float correctRotationAngle = 0f;
    public float rotationTolerance = 5f;
    
    [Header("Rotation Settings")]
    public float rotationStep = 45f;
    public float rotationDuration = 0.5f;
    
    [Header("Audio (Optional)")]
    public AudioClip rotateSound;
    
    private bool canRotate = false;
    private bool isRotating = false;
    private AudioSource audioSource;
    private float targetRotation;
    private float rotationProgress;
    private float startRotation;
    
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        targetRotation = transform.eulerAngles.y;
    }
    
    public void EnableRotation(bool enable)
    {
        canRotate = enable;
        Debug.Log($"[{gameObject.name}] Rotation {(enable ? "ENABLED" : "DISABLED")}. Current angle: {GetCurrentYRotation():F1}° | Target: {correctRotationAngle:F1}°");
    }
    
    void Interact()
    {
        if (!canRotate)
        {
            Debug.Log($"[{gameObject.name}] You need the {requiredKey} to rotate this statue!");
            return;
        }
        
        if (isRotating)
        {
            Debug.Log($"[{gameObject.name}] Already rotating, please wait...");
            return;
        }
        
        RotateStatue();
    }
    
    void RotateStatue()
    {
        startRotation = transform.eulerAngles.y;
        targetRotation = startRotation + rotationStep;
        isRotating = true;
        rotationProgress = 0f;
        
        if (audioSource != null && rotateSound != null)
        {
            audioSource.PlayOneShot(rotateSound);
        }
        
        Debug.Log($"[{gameObject.name}] Rotating to {targetRotation % 360f:F1}°");
    }
    
    void Update()
    {
        if (isRotating)
        {
            rotationProgress += Time.deltaTime / rotationDuration;
            
            if (rotationProgress >= 1f)
            {
                rotationProgress = 1f;
                isRotating = false;
            }
            
            float smoothProgress = Mathf.SmoothStep(0f, 1f, rotationProgress);
            float currentRotation = Mathf.LerpAngle(startRotation, targetRotation, smoothProgress);
            
            transform.rotation = Quaternion.Euler(
                transform.eulerAngles.x,
                currentRotation,
                transform.eulerAngles.z
            );
            
            if (!isRotating)
            {
                Debug.Log($"[{gameObject.name}] Rotation complete. Current Y: {GetCurrentYRotation():F1}°");
            }
        }
    }
    
    public bool IsCorrectlyRotated()
    {
        float currentRotation = GetCurrentYRotation();
        float difference = Mathf.Abs(Mathf.DeltaAngle(currentRotation, correctRotationAngle));
        bool isCorrect = difference <= rotationTolerance;
        
        Debug.Log($"[{gameObject.name}] IsCorrect: {isCorrect} | Current: {currentRotation:F1}° | Target: {correctRotationAngle:F1}° | Diff: {difference:F1}°");
        
        return isCorrect;
    }
    
    float GetCurrentYRotation()
    {
        return transform.eulerAngles.y;
    }
}
