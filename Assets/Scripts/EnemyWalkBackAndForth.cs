using UnityEngine;
using UnityEngine.AI;

public class EnemyWalkBackAndForth : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;

    private NavMeshAgent agent;
    private Transform currentTarget;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        currentTarget = pointA;
        agent.SetDestination(currentTarget.position);
    }

    void Update()
    {
        // If close enough to the target, switch
        if (!agent.pathPending && agent.remainingDistance < 0.2f)
        {
            currentTarget = (currentTarget == pointA) ? pointB : pointA;
            agent.SetDestination(currentTarget.position);
        }
    }
}
