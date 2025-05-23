using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyBase : MonoBehaviour
{
    protected GameManager gameManager;
    public NavMeshAgent agent;

    public Transform player;
    protected Vector3 playerPos;

    public LayerMask groundMask, playerMask, entityMask;
    [SerializeField] protected Animator animator;
    [SerializeField] protected Collider hitBox;//Collider that will react to environment and bullets from player
    public List<Collider> ragdollParts = new List<Collider>();

    //Stats
    public int damage;
    public float health;
    public float timeBetweenAttacks;
    public bool hasDied = false;

    //Patrolling stuff
    public Transform patrolAreaCenter;
    public Vector3 walkPoint;
    bool walkPointSet;
    public float patrolRange;

    //Attacking stuff
    protected bool alreadyAttacked;

    //States
    public Transform sightOrigin;
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange, playerInLineOfSight, isAggroed;
    public float aggroTime;
    public float chaseTime;

    protected void Awake() 
    {
        aggroTime = chaseTime;
        player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        SetRagdollParts();
    }

    protected void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    protected void Update() 
    {
        playerPos = player.GetComponent<PlayerMovement>().GetAimLocation();
        
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
        Vector2 point = Random.insideUnitCircle * patrolRange;
        walkPoint = new Vector3(point.x, 0, point.y) + patrolAreaCenter.position;

        if(Physics.Raycast(walkPoint, -transform.up, 1f, groundMask))
        {
            walkPointSet = true;
        }
    }

    protected void Huntin()
    {
        agent.SetDestination(playerPos);
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
        //make enemy chase player for some time when shot at
        aggroTime = -Mathf.Pow(chaseTime,2);

        if (health <= 0f && !hasDied)
        {
            hasDied = true;
            Die();
        }
    }

    protected void SetRagdollParts()
    {
        //Also disables ragdoll stuff
        Collider[] collider = this.gameObject.GetComponentsInChildren<Collider>();

        foreach(Collider c in collider)
        {
            if(c.gameObject != hitBox.gameObject)
            {
                c.isTrigger = true;
                c.attachedRigidbody.isKinematic = true;
                ragdollParts.Add(c);
            }
        }
    }

    protected void TurnOnRagdoll()
    {
        hitBox.enabled = false;
        animator.avatar = null;
        animator.enabled = false;
        foreach(Collider c in ragdollParts)
        {
            c.isTrigger = false;
            c.attachedRigidbody.isKinematic = false;
            c.attachedRigidbody.velocity = Vector3.zero;
        }
    }

    protected virtual void Die()
    {
        animator.SetBool("IsDead", true);
        Debug.Log("Enemy "+gameObject.name+" died!");
        agent.enabled = false;
        TurnOnRagdoll();
        Destroy(gameObject, 5f);
        gameManager.EnemyKilled();
        this.enabled = false;
    }

    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.cyan;
        if(patrolAreaCenter != null)
            Gizmos.DrawWireSphere(patrolAreaCenter.position, patrolRange);
    }
}
