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
        //Check for sight and attack ranges
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerMask);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);
        playerInMeleeRange = Physics.CheckSphere(transform.position, meleeRange, playerMask);

        if(!playerInSightRange && !playerInAttackRange && !playerInMeleeRange)
        {
            justEnteredMeleeRange = true;
            Vibin();
            hits = 0;
        }

        if(playerInSightRange && !playerInAttackRange && !playerInMeleeRange)
        {
            justEnteredMeleeRange = true;
            Huntin();
            hits = 0;
        }

        if(playerInSightRange && playerInAttackRange && !playerInMeleeRange)
        {
            justEnteredMeleeRange = true;
            Killin();
            hits = 0;
        }

        if(playerInSightRange && playerInAttackRange && playerInMeleeRange)
        {
            Sawin();
        }
    }

    private new void Killin()
    {
        //look at player but not on the y axis
        Vector3 lookPos = player.position - transform.position;
        lookPos.y = 0;
        transform.rotation = Quaternion.LookRotation(lookPos);
        
        if(!alreadyAttacked)
        {
            agent.SetDestination(transform.position);
            Attack();

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
            timePassed = 0f;
            hits = 0;
        }

        else if (timePassed >= standTime)
        {
            agent.SetDestination(player.position);
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
        Vector3 lookPos = player.position - transform.position;
        lookPos.y = 0;
        transform.rotation = Quaternion.LookRotation(lookPos);

        if(timePassed < timeBetweenMeleeAttacks + standTime)
        {
            //attack should hit 3 times in intervals of 0.4 seconds
            if(timePassed > (hits * (0.33f)) + standTime && hits < 3)
            {
                MeleeAttack();
                hits++;
            }
        }

        else
        {
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
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(attackSphere.transform.position, attackArea);
    }
}
