using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonController : MonoBehaviour
{
    // --- Movement ---
    public float walkSpeed = 5f;
    public float runSpeed = 9f;
    public float gravity = -24f;
    public float jumpHeight = 1.6f;

    [Header("Jump Settings")]
    public float jumpDelay = 1f;

    [Header("Ground Check")]
    public LayerMask groundLayers = ~0;     // set to "Default, Ground" etc.
    public float groundCheckRadius = 0.2f;  // foot probe radius
    public float groundCheckOffset = 0.05f; // how far below feet to probe

    // --- Camera ---
    public Transform cameraHolder;
    public float lookSensitivity = 2f;
    public float minPitch = -80f, maxPitch = 80f;

    // --- Animator ---
    public string speedParam = "Speed";
    public string groundedBool = "IsGrounded";
    public string jumpTrigger = "Jump";

    CharacterController cc;
    Animator animator;
    Vector3 velocity;
    float yaw, pitch;

    // Jump state
    bool jumpQueued = false;
    float jumpTimer = 0f;
    bool jumpArmed = true;
    bool jumpHeld = false;

    // cached grounded state
    bool groundedNow, groundedPrev;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        if (animator) animator.applyRootMotion = false;

        if (!cameraHolder)
        {
            var cam = GetComponentInChildren<Camera>();
            if (cam) cameraHolder = cam.transform.parent ? cam.transform.parent : cam.transform;
        }
        if (cameraHolder) yaw = cameraHolder.eulerAngles.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // input
        bool press = Input.GetKeyDown(KeyCode.Space);
        bool release = Input.GetKeyUp(KeyCode.Space);
        jumpHeld = Input.GetKey(KeyCode.Space);
        if (release) jumpArmed = true;

        Look();

        // --- PLANAR MOVE FIRST ---
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(h, 0f, v).normalized;
        if (input.sqrMagnitude > 0.0001f && cameraHolder)
        {
            Vector3 f = cameraHolder.forward; f.y = 0f; f.Normalize();
            Vector3 r = cameraHolder.right; r.y = 0f; r.Normalize();
            Vector3 moveDir = (f * v + r * h).normalized;

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir), 10f * Time.deltaTime);
            float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
            cc.Move(moveDir * speed * Time.deltaTime);
        }

        // --- VERTICAL MOVE / GRAVITY ---
        groundedPrev = groundedNow;
        groundedNow = FastGrounded(); // our own immediate ground probe

        if (groundedNow && velocity.y < 0f)
            velocity.y = -4f; // stronger stick to ground to snap contact

        // queue jump (don’t lift yet)
        if (groundedNow && press && !jumpQueued && jumpArmed)
        {
            jumpQueued = true;
            jumpTimer = Mathf.Max(0f, jumpDelay);
            if (animator) { animator.ResetTrigger(jumpTrigger); animator.SetTrigger(jumpTrigger); }
            jumpArmed = false;
        }

        // apply gravity and vertical move
        velocity.y += gravity * Time.deltaTime;
        CollisionFlags flags = cc.Move(new Vector3(0f, velocity.y, 0f) * Time.deltaTime);

        // also consider CharacterController’s Below flag, but our probe is primary
        if ((flags & CollisionFlags.Below) != 0) groundedNow = true;

        // liftoff handling (after Move so groundedNow is most current)
        HandleJumpDelay();

        // Animator params
        if (animator)
        {
            float mag = new Vector2(h, v).magnitude;
            bool running = Input.GetKey(KeyCode.LeftShift) && mag > 0f;
            float animSpeed = mag * (running ? 1f : 0.5f);
            animator.SetFloat(speedParam, animSpeed, 0.1f, Time.deltaTime);
            animator.SetBool(groundedBool, groundedNow);
        }

        // re-arm next jump when landed and key released
        if (groundedNow && !jumpHeld) jumpArmed = true;
    }

    void Look()
    {
        if (!cameraHolder) return;
        float mx = Input.GetAxis("Mouse X") * lookSensitivity;
        float my = Input.GetAxis("Mouse Y") * lookSensitivity;
        yaw += mx; pitch -= my;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        cameraHolder.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    void HandleJumpDelay()
    {
        if (!jumpQueued) return;
        jumpTimer -= Time.deltaTime;
        if (jumpTimer <= 0f)
        {
            if (groundedNow) // use our immediate grounded state
                velocity.y = Mathf.Sqrt(2f * jumpHeight * -gravity);

            jumpQueued = false; // consume regardless
        }
    }

    // Immediate ground check at feet (faster than waiting on isGrounded)
    bool FastGrounded()
    {
        // Build a small sphere under the capsule’s feet
        var b = cc.bounds;
        Vector3 feet = new Vector3(b.center.x, b.min.y + groundCheckOffset, b.center.z);
        return Physics.CheckSphere(feet, groundCheckRadius, groundLayers, QueryTriggerInteraction.Ignore);
    }

    // visualize the ground probe in editor
    void OnDrawGizmosSelected()
    {
        if (!cc) cc = GetComponent<CharacterController>();
        var b = cc.bounds;
        Vector3 feet = new Vector3(b.center.x, b.min.y + groundCheckOffset, b.center.z);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(feet, groundCheckRadius);
    }
}
