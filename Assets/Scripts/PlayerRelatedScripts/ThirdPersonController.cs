using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ThirdPersonMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 9f;
    public float crouchSpeed = 2.5f;

    [Header("Jumping")]
    public float jumpHeight = 1.6f;
    public float gravity = -9.81f;
    public float jumpLockout = 0.08f;

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
    public string crouchBool = "IsCrouching";
    public string crouchSpeedParam = "CrouchSpeed";

    [Header("Climbing")]
    public SimpleMantle mantle;   // drag your SimpleMantle here in the inspector

    CharacterController cc;
    Vector3 velocity;
    float yaw, pitch;
    float jumpTimer = 0f;

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

        bool grounded = cc.isGrounded;
        bool isCrouching = Input.GetKey(KeyCode.LeftControl);
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) && !isCrouching;

        float targetSpeed = isCrouching ? crouchSpeed : (isSprinting ? sprintSpeed : walkSpeed);

        if (grounded && jumpTimer <= 0f && velocity.y < 0f)
            velocity.y = -2f;

        // MANTLE OR JUMP
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // First, try to mantle
            if (mantle != null && mantle.TryStartMantleOnSpace())
            {
                // Mantle started, SimpleMantle will disable this script,
                // so stop Update here for this frame
                return;
            }

            // If no ledge, do normal jump if grounded
            if (grounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                jumpTimer = jumpLockout;
                if (animator)
                {
                    animator.ResetTrigger(jumpTrigger);
                    animator.SetTrigger(jumpTrigger);
                }
            }
        }

        velocity.y += gravity * Time.deltaTime;

        Vector3 total = moveDir * targetSpeed + new Vector3(0f, velocity.y, 0f);
        CollisionFlags flags = cc.Move(total * Time.deltaTime);
        grounded = (flags & CollisionFlags.Below) != 0;

        if (animator)
        {
            float mag = new Vector2(h, v).magnitude;
            float animSpeed = mag * (isSprinting ? 1f : 0.5f);
            animator.SetFloat(speedParam, animSpeed, 0.1f, Time.deltaTime);
            animator.SetBool(groundedBool, grounded);
            animator.SetBool(crouchBool, isCrouching);
            animator.SetFloat(crouchSpeedParam, isCrouching ? mag : 0f, 0.1f, Time.deltaTime);
        }

        if (jumpTimer > 0f) jumpTimer -= Time.deltaTime;
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
