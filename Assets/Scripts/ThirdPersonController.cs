using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class SimpleThirdPersonController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 9f;

    [Header("Jumping")]
    public float jumpHeight = 1.6f;
    public float gravity = -9.81f;
    public float jumpStickLockout = 0.08f;

    [Header("Camera")]
    public Transform cameraPivot;
    public float lookSensitivity = 2f;
    public float minPitch = -60f;
    public float maxPitch = 75f;

    [Header("Animator")]
    public Animator animator;
    public string speedParam = "Speed";
    public string groundedBool = "IsGrounded";
    public string jumpTrigger = "Jump";

    CharacterController cc;
    Vector3 velocity;
    float yaw, pitch;
    float jumpLockTimer = 0f;
    bool isJumping = false;

    void Awake()
    {
        cc = GetComponent<CharacterController>();

        if (!animator) animator = GetComponentInChildren<Animator>();
        if (animator) animator.applyRootMotion = false;

        if (!cameraPivot)
        {
            var cam = Camera.main;
            if (cam) cameraPivot = cam.transform.parent ? cam.transform.parent : cam.transform;
        }
        if (cameraPivot) yaw = cameraPivot.eulerAngles.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Look();

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 input = Vector3.ClampMagnitude(new Vector3(h, 0f, v), 1f);

        Vector3 fwd = cameraPivot ? cameraPivot.forward : Vector3.forward;
        Vector3 rgt = cameraPivot ? cameraPivot.right : Vector3.right;
        fwd.y = 0f; rgt.y = 0f; fwd.Normalize(); rgt.Normalize();

        Vector3 moveDir = fwd * input.z + rgt * input.x;

        if (moveDir.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDir), 12f * Time.deltaTime);

        float targetSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;

        bool grounded = cc.isGrounded;

        // Stick to ground unless jumping
        if (grounded && jumpLockTimer <= 0f && velocity.y < 0f)
        {
            velocity.y = -2f;
            isJumping = false;
        }

        // Jump input
        if (grounded && Input.GetKeyDown(KeyCode.Space))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jumpLockTimer = jumpStickLockout;
            isJumping = true;
            if (animator)
            {
                animator.ResetTrigger(jumpTrigger);
                animator.SetTrigger(jumpTrigger);
            }
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Final movement vector
        Vector3 total = moveDir * targetSpeed + new Vector3(0f, velocity.y, 0f);
        CollisionFlags flags = cc.Move(total * Time.deltaTime);

        // Update grounded state
        grounded = (flags & CollisionFlags.Below) != 0;

        // Animator updates
        if (animator)
        {
            float mag = new Vector2(h, v).magnitude;
            bool running = Input.GetKey(KeyCode.LeftShift) && mag > 0f;
            float animSpeed = mag * (running ? 1f : 0.5f);
            animator.SetFloat(speedParam, animSpeed, 0.1f, Time.deltaTime);
            animator.SetBool(groundedBool, grounded);
        }

        // Timers
        if (jumpLockTimer > 0f) jumpLockTimer -= Time.deltaTime;
    }

    void Look()
    {
        if (!cameraPivot) return;
        float mx = Input.GetAxis("Mouse X") * lookSensitivity;
        float my = Input.GetAxis("Mouse Y") * lookSensitivity;
        yaw += mx;
        pitch -= my;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        cameraPivot.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}
