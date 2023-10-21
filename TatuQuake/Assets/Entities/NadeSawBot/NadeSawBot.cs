using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NadeSawBot : EnemyBase
{
    public delegate void FireAction();
    public static event FireAction OnFired;
    [SerializeField] private GameObject attackSphere;
    [SerializeField] private float attackArea = 1f;
    [SerializeField] float impactForce = 1f;
    [SerializeField] EnemyNade EnemyNade;
    [SerializeField] GameObject startPosition;

    public float standTime = 0.8f;
    private float timePassed = 0f;

    //Melee state
    public int meleeDamage;
    public float meleeRange;
    public bool alreadyMeleeAttacked;
    public bool playerInMeleeRange;
    public float timeBetweenMeleeAttacks;
    private int hits = 0;
    private bool justEnteredMeleeRange = true;
    

    // Update is called once per frame
    private new void Update() 
    {
        aggroTime += Time.deltaTime;

        playerPos = player.GetComponent<PlayerMovement>().GetAimLocation();

        //Check for sight and attack ranges
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerMask);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);
        playerInMeleeRange = Physics.CheckSphere(transform.position, meleeRange, playerMask);

        //Line of sight
        if(playerInSightRange)
        {
            RaycastHit hit;

            if(Physics.Raycast(sightOrigin.position, playerPos - sightOrigin.position, out hit,  Mathf.Infinity, ~entityMask))
            {
                playerInLineOfSight = hit.collider.CompareTag("Player");
            }
        }

        if(((!playerInSightRange && !playerInAttackRange && !playerInMeleeRange) || !playerInLineOfSight) && aggroTime >= chaseTime)
        {
            animator.SetBool("IsAttacking", false);
            animator.SetBool("IsSawing", false);
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsRunning", false);
            justEnteredMeleeRange = true;
            Vibin();
            hits = 0;
        }

        if((playerInSightRange && !playerInAttackRange && !playerInMeleeRange && playerInLineOfSight) || aggroTime <= chaseTime && ((!playerInAttackRange && !playerInMeleeRange) || !playerInLineOfSight))
        {
            animator.SetBool("IsAttacking", false);
            animator.SetBool("IsSawing", false);
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", true);
            justEnteredMeleeRange = true;
            if(playerInSightRange && playerInLineOfSight)
            {
                aggroTime = 0f;
            }
            Huntin();
            hits = 0;
        }

        if(playerInSightRange && playerInAttackRange && !playerInMeleeRange && playerInLineOfSight)
        {
            animator.SetBool("IsSawing", false);
            animator.SetBool("IsWalking", false);
            justEnteredMeleeRange = true;
            if(playerInSightRange && playerInLineOfSight)
            {
                aggroTime = 0f;
            }
            Killin();
            hits = 0;
        }

        if(playerInSightRange && playerInAttackRange && playerInMeleeRange && playerInLineOfSight)
        {
            animator.SetBool("IsAttacking", false);
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);
            if(playerInSightRange && playerInLineOfSight)
            {
                aggroTime = 0f;
            }
            Sawin();
        }
    }

    private new void Killin()
    {
        //look at player but not on the y axis
        Vector3 lookPos = playerPos - transform.position;
        lookPos.y = 0;
        transform.rotation = Quaternion.LookRotation(lookPos);
        
        if(!alreadyAttacked)
        {
            animator.SetBool("IsRunning", false);
            animator.SetBool("IsAttacking",true);
            agent.SetDestination(transform.position);
            Attack();

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
            timePassed = 0f;
            hits = 0;
        }

        else if (timePassed >= standTime)
        {
            animator.SetBool("IsRunning", true);
            animator.SetBool("IsAttacking",false);
            agent.SetDestination(playerPos);
        }
        timePassed += Time.deltaTime;
    }

    private void Sawin()
    {
        if(justEnteredMeleeRange)
        {
            justEnteredMeleeRange = false;
            timePassed = 0.8f;
        }

        //look at player but not on the y axis
        Vector3 lookPos = playerPos - transform.position;
        lookPos.y = 0;
        transform.rotation = Quaternion.LookRotation(lookPos);

        if(timePassed < timeBetweenMeleeAttacks + standTime)
        {
            //attack should hit 3 times in intervals of 0.4 seconds
            if(timePassed > (hits * (0.33f)) + standTime && hits < 3)
            {
                animator.SetBool("IsSawing", true);
                if(hits == 0) SoundManager.instance.PlaySound(SoundManager.Sound.SawAttack);
                MeleeAttack();
                hits++;
            }

        }

        else
        {
            animator.SetBool("IsSawing", false);
            timePassed = 0.8f;
            hits = 0;
        }
        timePassed += Time.deltaTime;
    }

    protected override void Attack()
    {
        SoundManager.instance.PlaySound(SoundManager.Sound.NLShot);

        Vector3 nadeStartPos = startPosition.transform.position;
        Quaternion nadeStartRot = startPosition.transform.rotation;
        EnemyNade projectile = Instantiate(EnemyNade, startPosition.transform.position, nadeStartRot);
        projectile.SetDmg(damage);
        projectile.SetFrc(impactForce);
        projectile.SetFrwd(player.transform.position - nadeStartPos);
        OnFired();
    }

    protected new void ResetAttack()
    {
        alreadyAttacked = false;
        animator.SetBool("IsAttacking",false);
    }

    private void MeleeAttack()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, meleeRange);
        foreach (Collider nearbyObj in colliders)
        {
            PlayerMovement player = nearbyObj.GetComponent<PlayerMovement>();
            if(player != null)
            {
                player.TakeDamage(meleeDamage);
            }
        }
    }

    private void ResetMeleeAttack()
    {
        alreadyMeleeAttacked = false;
    }

    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(attackSphere.transform.position, attackArea);
        Gizmos.color = Color.cyan;
        if(patrolAreaCenter != null)
            Gizmos.DrawWireSphere(patrolAreaCenter.position, patrolRange);
    }
}
