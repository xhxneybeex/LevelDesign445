using UnityEngine;
using System.Collections;

public class SimpleMantle : MonoBehaviour
{
    [Header("Refs")]
    public CharacterController controller;
    public MonoBehaviour movementScript;   // your movement controller to disable during mantle
    public Transform probeOrigin;          // chest height
    public LayerMask geometryMask = ~0;    // layers considered for walls and tops

    [Header("Probe")]
    public float wallDistance = 0.7f;
    public float topProbeUp = 1.0f;
    public float topProbeDown = 2.0f;
    public float handBackFromWall = 0.3f;

    [Header("Mantle Motion")]
    public float snapSpeed = 12f;
    public float mantleTime = 0.45f;
    public Vector3 mantleOffset = new Vector3(0f, 1.0f, 0.6f);

    enum State { Normal, Hanging, Mantling }
    State state = State.Normal;

    Vector3 hangPoint, standPoint, wallNormal;

    // Call this from your movement code when Space is pressed
    public bool TryStartMantleOnSpace()
    {
        if (state != State.Normal) return false;

        if (FindLedge(out hangPoint, out standPoint, out wallNormal))
        {
            StartCoroutine(SnapToHang());
            return true; // consumed Space, do not jump
        }
        return false;    // no ledge, allow jump
    }

    bool FindLedge(out Vector3 outHang, out Vector3 outStand, out Vector3 outNormal)
    {
        outHang = outStand = outNormal = Vector3.zero;

        // 1) forward ray to find a near-vertical wall
        if (!Physics.Raycast(new Ray(probeOrigin.position, transform.forward),
                             out RaycastHit wallHit, wallDistance, geometryMask,
                             QueryTriggerInteraction.Ignore)) return false;

        // wall should be roughly vertical
        if (Mathf.Abs(Vector3.Dot(wallHit.normal, Vector3.up)) > 0.3f) return false;

        // 2) from above hit point, raycast downward to find the top surface
        Vector3 downStart = wallHit.point + Vector3.up * topProbeUp - wallHit.normal * 0.05f;
        if (!Physics.Raycast(downStart, Vector3.down, out RaycastHit topHit, topProbeDown, geometryMask,
                             QueryTriggerInteraction.Ignore)) return false;

        // top should be standable
        if (Vector3.Angle(topHit.normal, Vector3.up) > 50f) return false;

        outNormal = wallHit.normal;

        // hands just below edge, slightly away from wall
        Vector3 edge = topHit.point;
        outHang = edge - outNormal * handBackFromWall;
        outHang.y -= 0.15f;

        // final stand point relative to facing away from the wall
        Vector3 away = Vector3.ProjectOnPlane(-outNormal, Vector3.up).normalized;
        Quaternion faceAway = Quaternion.LookRotation(away, Vector3.up);
        outStand = outHang + faceAway * mantleOffset;

        // clearance check for capsule at stand point
        float h = controller.height;
        float r = controller.radius;
        if (Physics.CheckCapsule(outStand + Vector3.up * 0.1f,
                                 outStand + Vector3.up * (h - 0.1f),
                                 r * 0.95f, geometryMask, QueryTriggerInteraction.Ignore))
            return false;

        return true;
    }

    IEnumerator SnapToHang()
    {
        state = State.Mantling;
        ToggleMove(false);

        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        Quaternion endRot = Quaternion.LookRotation(Vector3.ProjectOnPlane(-wallNormal, Vector3.up), Vector3.up);

        float dist = Vector3.Distance(startPos, hangPoint);
        float dur = Mathf.Max(0.05f, dist / snapSpeed);

        controller.enabled = false;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / dur;
            transform.position = Vector3.Lerp(startPos, hangPoint, t);
            transform.rotation = Quaternion.Slerp(startRot, endRot, t);
            yield return null;
        }
        controller.enabled = true;

        // instant hoist when Space is pressed again, or auto hoist here:
        StartCoroutine(MantleUp());
    }

    IEnumerator MantleUp()
    {
        state = State.Mantling;

        Vector3 start = transform.position;
        controller.enabled = false;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / mantleTime;
            float k = t * t * (3f - 2f * t); // smoothstep
            transform.position = Vector3.Lerp(start, standPoint, k);
            yield return null;
        }
        controller.enabled = true;

        ExitMantle();
    }

    void ExitMantle()
    {
        state = State.Normal;
        ToggleMove(true);
    }

    void ToggleMove(bool on)
    {
        if (movementScript) movementScript.enabled = on;
    }
}
