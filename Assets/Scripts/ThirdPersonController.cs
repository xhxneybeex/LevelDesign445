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

    [Header("Mantle Animator Params")]
    public string mantleBool = "IsMantling";
    public string mantleTrigger = "MantleStart";
    public bool useRootMotionDuringMantle = false; // turn on if your mantle clip moves the character

    // ---------------- MANTLE SETTINGS ----------------
    [Header("Mantle, assign ProbeOrigin and layers")]
    public Transform probeOrigin;               // empty at chest height
    public LayerMask geometryMask = ~0;         // what counts as walls and tops

    [Tooltip("Max distance to detect a wall in front")]
    public float mantleWallDistance = 0.7f;
    [Tooltip("How high above the wall hit to start the downward ray")]
    public float topProbeUp = 1.0f;
    [Tooltip("How far downward to search for the top")]
    public float topProbeDown = 2.0f;
    [Tooltip("Hands sit this far back from the wall")]
    public float handBackFromWall = 0.3f;

    [Header("Mantle Motion")]
    public float snapToHangSpeed = 12f;         // movement speed when snapping to hang
    public float mantleTime = 0.45f;            // duration of hoist (match to clip if using root motion)
    public Vector3 mantleOffset = new Vector3(0f, 1.0f, 0.6f); // from hang to final stand

    CharacterController cc;
    Vector3 velocity;
    float yaw, pitch;
    float jumpLockTimer = 0f;
    bool isJumping = false;

    enum MantleState { None, Hanging, Mantling }
    MantleState mantleState = MantleState.None;
    Vector3 hangPoint, standPoint, wallNormal;

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
        // Camera look still works during mantle
        Look();

        // If we are mantling, halt normal locomotion/physics update
        if (mantleState != MantleState.None)
        {
            // Keep animator informed
            if (animator) animator.SetBool(groundedBool, false);
            return;
        }

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

        // Space: try mantle first, if not possible do a normal jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (TryStartMantle())  // consumed Space, do not jump
            {
                // Animator will be triggered inside the mantle coroutine
            }
            else if (grounded)     // no ledge, do normal jump
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

    // ---------------- MANTLE LOGIC ----------------

    bool TryStartMantle()
    {
        // Allow mantle whether grounded or not, as long as a ledge is in front
        if (!FindLedge(out hangPoint, out standPoint, out wallNormal)) return false;

        StartCoroutine(SnapToHangAndMantle());
        return true;
    }

    bool FindLedge(out Vector3 outHang, out Vector3 outStand, out Vector3 outNormal)
    {
        outHang = outStand = outNormal = Vector3.zero;

        // Fallback if probeOrigin is missing
        Vector3 probePos = probeOrigin ? probeOrigin.position : (transform.position + Vector3.up * 1.2f);

        // 1) forward ray to find near-vertical wall
        if (!Physics.Raycast(new Ray(probePos, transform.forward),
                             out RaycastHit wallHit, mantleWallDistance, geometryMask,
                             QueryTriggerInteraction.Ignore))
            return false;

        // Surface should be roughly vertical
        if (Mathf.Abs(Vector3.Dot(wallHit.normal, Vector3.up)) > 0.3f) return false;

        // 2) from above that contact, cast downward to find top surface
        Vector3 downStart = wallHit.point + Vector3.up * topProbeUp - wallHit.normal * 0.05f;
        if (!Physics.Raycast(downStart, Vector3.down, out RaycastHit topHit, topProbeDown, geometryMask,
                             QueryTriggerInteraction.Ignore))
            return false;

        // Top should be standable
        if (Vector3.Angle(topHit.normal, Vector3.up) > 50f) return false;

        outNormal = wallHit.normal;

        // Hands just below edge, slightly back from wall
        Vector3 edge = topHit.point;
        outHang = edge - outNormal * handBackFromWall;
        outHang.y -= 0.15f;

        // Final stand point relative to facing away from wall
        Vector3 away = Vector3.ProjectOnPlane(-outNormal, Vector3.up).normalized;
        Quaternion faceAway = Quaternion.LookRotation(away, Vector3.up);
        outStand = outHang + faceAway * mantleOffset;

        // Capsule clearance check at stand point
        float h = cc.height;
        float r = cc.radius;
        if (Physics.CheckCapsule(outStand + Vector3.up * 0.1f,
                                 outStand + Vector3.up * (h - 0.1f),
                                 r * 0.95f, geometryMask, QueryTriggerInteraction.Ignore))
            return false;

        return true;
    }

    System.Collections.IEnumerator SnapToHangAndMantle()
    {
        mantleState = MantleState.Mantling;

        // Animator: enter mantle
        if (animator)
        {
            animator.SetBool(mantleBool, true);
            if (!string.IsNullOrEmpty(mantleTrigger))
            {
                animator.ResetTrigger(mantleTrigger);
                animator.SetTrigger(mantleTrigger);
            }
            if (useRootMotionDuringMantle) animator.applyRootMotion = true;
        }

        // Face the wall while snapping
        Quaternion startRot = transform.rotation;
        Quaternion endRot = Quaternion.LookRotation(Vector3.ProjectOnPlane(-wallNormal, Vector3.up), Vector3.up);

        Vector3 startPos = transform.position;
        float dist = Vector3.Distance(startPos, hangPoint);
        float dur = Mathf.Max(0.05f, dist / snapToHangSpeed);

        // Pause CharacterController while we place the character
        cc.enabled = false;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / dur;
            transform.position = Vector3.Lerp(startPos, hangPoint, t);
            transform.rotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }

        // Hoist up
        if (useRootMotionDuringMantle)
        {
            // Let the animation drive the root for mantleTime seconds
            float timer = 0f;
            while (timer < mantleTime)
            {
                timer += Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            // Scripted hoist
            Vector3 hoistStart = transform.position;
            float t2 = 0f;
            while (t2 < 1f)
            {
                t2 += Time.deltaTime / mantleTime;
                float k = t2 * t2 * (3f - 2f * t2); // smoothstep
                transform.position = Vector3.Lerp(hoistStart, standPoint, k);
                yield return null;
            }
        }

        // Done, resume normal control
        cc.enabled = true;
        mantleState = MantleState.None;
        velocity = Vector3.zero; // clear any carry-over

        if (animator)
        {
            animator.SetBool(mantleBool, false);
            if (useRootMotionDuringMantle) animator.applyRootMotion = false;
        }
    }

    // Optional gizmo to see the forward probe in editor
    void OnDrawGizmosSelected()
    {
        if (!probeOrigin) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(probeOrigin.position, probeOrigin.position + transform.forward * mantleWallDistance);
    }
}
