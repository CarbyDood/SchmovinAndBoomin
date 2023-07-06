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
        playerPos = player.GetComponent<PlayerMovement>().GetAimLocation();
        //Check for sight and attack ranges
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerMask);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);

        if(!playerInSightRange && !playerInAttackRange)
        {
            Vibin();
        }

        if(playerInSightRange && !playerInAttackRange)
        {
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
            Attack();

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }

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
