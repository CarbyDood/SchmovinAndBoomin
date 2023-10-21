using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightBot : EnemyBase
{
    [SerializeField] private GameObject attackSphere;
    [SerializeField] private float attackArea = 1f;
    [SerializeField] float impactForce = 1f;
    [SerializeField] private KnightProjectile projectile;
    [SerializeField] GameObject startPosition;

    [SerializeField] private float standTime = 0.8f;
    [SerializeField] private float attackDelay = 1f;
    private float timePassed = 0f;

    //Melee state
    public int meleeDamage;
    public float meleeRange;
    public bool alreadyMeleeAttacked;
    public bool playerInMeleeRange;
    public float timeBetweenMeleeAttacks;

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
            animator.SetBool("IsShooting", false);
            animator.SetBool("IsSlashing", false);
            animator.SetBool("IsWalking", true);
            animator.SetBool("IsRunning", false);
            Vibin();
            timePassed = 0f;
        }

        if((playerInSightRange && !playerInAttackRange && !playerInMeleeRange && playerInLineOfSight) || aggroTime <= chaseTime && ((!playerInAttackRange && !playerInMeleeRange) || !playerInLineOfSight))
        {
            animator.SetBool("IsShooting", false);
            animator.SetBool("IsSlashing", false);
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", true);
            if(playerInSightRange && playerInLineOfSight)
            {
                aggroTime = 0f;
            }
            Huntin();
            timePassed = 0f;
        }

        if(playerInSightRange && playerInAttackRange && !playerInMeleeRange && playerInLineOfSight)
        {
            animator.SetBool("IsSlashing", false);
            animator.SetBool("IsWalking", false);
            if(playerInSightRange && playerInLineOfSight)
            {
                aggroTime = 0f;
            }
            Killin();
        }

        if(playerInSightRange && playerInAttackRange && playerInMeleeRange && playerInLineOfSight)
        {
            animator.SetBool("IsShooting", false);
            animator.SetBool("IsWalking", false);
            animator.SetBool("IsRunning", false);
            if(playerInSightRange && playerInLineOfSight)
            {
                aggroTime = 0f;
            }
            Slicin();
            timePassed = 0f;
        }
    }

    private new void Killin()
    {
        if(timePassed < standTime + attackDelay)
        {
            animator.SetBool("IsRunning", false);
        }
        agent.SetDestination(transform.position);
        //look at player but not on the y axis
        Vector3 lookPos = playerPos - transform.position;
        lookPos.y = 0;
        transform.rotation = Quaternion.LookRotation(lookPos);
        
        if(!alreadyAttacked && timePassed > attackDelay)
        {
            animator.SetBool("IsShooting", true);
            Attack();

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
            Invoke(nameof(ResetTimer), timeBetweenAttacks);
        }

        else if (timePassed >= standTime + attackDelay)
        {
            animator.SetBool("IsShooting", false);
            animator.SetBool("IsRunning", true);
            agent.SetDestination(playerPos);
        }
        timePassed += Time.deltaTime;
    }

    private void Slicin()
    {
        //look at player but not on the y axis
        Vector3 lookPos = playerPos - transform.position;
        lookPos.y = 0;
        transform.rotation = Quaternion.LookRotation(lookPos);

        if(!alreadyMeleeAttacked)
        {
            animator.SetBool("IsSlashing",true);
            MeleeAttack();
            
            alreadyMeleeAttacked = true;
            Invoke(nameof(ResetMeleeAttack), timeBetweenMeleeAttacks);
        }

    }

    protected override void Attack()
    {
        //SoundManager.instance.PlaySound(SoundManager.Sound.SwordBeamAttack);

        Vector3 projStartPos = startPosition.transform.position;
        Quaternion projStartRot = startPosition.transform.rotation;
        float xOffSet = -0.95f;
        float adjust = Mathf.Abs(xOffSet);
        for(int i = 0; i < 3; i++)
        {
            KnightProjectile beam = Instantiate(projectile, projStartPos, projStartRot);
            beam.SetDmg(damage);
            beam.SetFrc(impactForce);
            Vector3 direction = playerPos - startPosition.transform.position;
            direction.x += xOffSet;
            beam.SetFrwd((direction));
            xOffSet += adjust;
        }
    }

    protected new void ResetAttack()
    {
        alreadyAttacked = false;
        animator.SetBool("IsShooting",false);
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
        animator.SetBool("IsSlashing",false);
    }

    private void ResetTimer()
    {
        timePassed = 0f;
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
