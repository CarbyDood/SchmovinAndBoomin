using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderBot : EnemyBase
{
    [SerializeField] private SpiderProjectile projectile;
    [SerializeField] private float impactForce = 10f;
    [SerializeField] GameObject startPosition;

    private float timePassed = 0f;

    // Update is called once per frame
    new void Update()
    {
        //Animation stuff
        if(agent.velocity.magnitude >= 0.1f)
            animator.SetBool("IsMoving",true);
        else
            animator.SetBool("IsMoving",false);
        
        playerPos = player.GetComponent<PlayerMovement>().GetAimLocation();
        //Check for sight and attack ranges
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerMask);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);

        if(!playerInSightRange && !playerInAttackRange)
        {
            animator.SetBool("IsAttacking",false);
            Vibin();
        }

        if(playerInSightRange && !playerInAttackRange)
        {
            animator.SetBool("IsAttacking",false);
            Huntin();
        }

        if(playerInSightRange && playerInAttackRange)
        {
            Killin();
        }
    }

    private new void Killin()
    {
        agent.SetDestination(transform.position);
        //look at player but not on the y axis
        Vector3 lookPos = playerPos - transform.position;
        lookPos.y = 0;
        transform.rotation = Quaternion.LookRotation(lookPos);
        
        if(!alreadyAttacked)
        {
            animator.SetBool("IsAttacking",true);
            Attack();

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }

        else{animator.SetBool("IsAttacking",false);}

        timePassed += Time.deltaTime;
    }

    protected override void Attack()
    {
        Vector3 projStartPos = startPosition.transform.position;
        Quaternion projStartRot = transform.rotation;
        SpiderProjectile SpiderBomb = Instantiate(projectile, projStartPos, projStartRot);
        SpiderBomb.SetDmg(damage);
        SpiderBomb.SetFrc(impactForce);
    }
}
