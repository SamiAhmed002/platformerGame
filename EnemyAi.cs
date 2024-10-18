using UnityEngine;
using UnityEngine.AI;  // Required for NavMeshAgent

public class EnemyAI : MonoBehaviour
{
    public Transform[] patrolPoints;   // Points for patrolling
    public Transform player;           // Reference to the player
    public float chaseRange = 10f;     // Range within which the enemy starts chasing
    public float attackRange = 2f;     // Distance to attack the player

    private int currentPatrolIndex;
    private NavMeshAgent navMeshAgent;
    private bool isChasing = false;
    
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseRange)
        {
            isChasing = true;
        }
        else
        {
            isChasing = false;
        }

        if (isChasing)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }

        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
        }
    }

    void Patrol()
    {
        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            navMeshAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    void ChasePlayer()
    {
        navMeshAgent.SetDestination(player.position);
    }

    void AttackPlayer()
    {
        // Code for attacking the player (e.g., reduce player health)
        Debug.Log("Attacking the player!");
    }
}
