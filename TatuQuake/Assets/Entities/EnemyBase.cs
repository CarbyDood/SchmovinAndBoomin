using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask groundMask, playerMask;

    //Stats
    public int damage;
    public float health;
    public float timeBetweenAttacks;

    //Patrolling stuff
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking stuff
    protected bool alreadyAttacked;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    protected void Awake() 
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    protected void Update() 
    {
        //Check for sight and attack ranges
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerMask);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);

        if(!playerInSightRange && !playerInAttackRange)
            Vibin();
        if(playerInSightRange && !playerInAttackRange)
            Huntin();
        if(playerInSightRange && playerInAttackRange)
            Killin();
    }

    protected void Vibin()
    {
        if(!walkPointSet)
            SearchWalkPoint();

        if(walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Reached point
        if(distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    protected void SearchWalkPoint()
    {
        //Set Random points in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if(Physics.Raycast(walkPoint, -transform.up, 1f, groundMask))
            walkPointSet = true;
    }

    protected void Huntin()
    {
        agent.SetDestination(player.position);
    }

    protected void Killin()
    {
        //don't want enemy to move
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if(!alreadyAttacked)
        {
            Attack();

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    //abstract method!! doesn't do anything, make an attack() method override for every enemy!
    protected abstract void Attack();
        
    protected void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(float dmg)
    {
        health -= dmg;

        if (health <= 0f)
        {
            Die();
        }
    }

    protected void Die()
    {
        Debug.Log("Enemy "+gameObject.name+" died!");
        agent.enabled = false;
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        if(rb == null)
            rb = gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
        Destroy(gameObject, 5f);
        this.enabled = false;
    }

    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
