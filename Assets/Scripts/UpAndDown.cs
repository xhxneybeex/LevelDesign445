using UnityEngine;
using System.Collections;

public class UpAndDown : MonoBehaviour
{
    public float moveDistance = 3f;   // How far to move upward
    public float speed = 3f;          // Movement speed

    private bool isOpen = false;
    private Vector3 closedPos;
    private Vector3 openPos;
    private Coroutine currentRoutine;

    void Start()
    {
        closedPos = transform.position;
        openPos = closedPos + Vector3.up * moveDistance;
    }

    public void Interact()
    {
        Debug.Log("Interact() CALLED on door!");

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(MoveDoor());
    }

    IEnumerator MoveDoor()
    {
        Vector3 target = isOpen ? closedPos : openPos;
        isOpen = !isOpen;

        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
    }
}
