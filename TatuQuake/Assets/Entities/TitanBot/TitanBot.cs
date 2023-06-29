using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitanBot : EnemyBase
{
    [SerializeField] float impactForce = 30f;
    private float range = 40f;
    [SerializeField] protected ParticleSystem lightingAttackEffect;
    [SerializeField] protected GameObject impactEffect;
    [SerializeField] protected float attackDelay = 1f;
    [SerializeField] protected float hitInterval = 0.3f;
    private float timePassed = 0f;


    private new void Update() 
    {
        playerPos = player.GetComponent<PlayerMovement>().GetAimLocation();
        //Check for sight and attack ranges
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerMask);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);

        if(!playerInSightRange && !playerInAttackRange)
        {
            Vibin();
            lightingAttackEffect.Stop();
        }

        if(playerInSightRange && !playerInAttackRange)
        {
            Huntin();
            lightingAttackEffect.Stop();
        }

        if(playerInSightRange && playerInAttackRange)
        {
            Killin();
        }
        timePassed += Time.deltaTime;
    }

    private new void Killin()
    {
        //look at player but not on the y axis
        agent.SetDestination(transform.position);
        Vector3 lookPos = playerPos - transform.position;
        lookPos.y = 0;
        transform.rotation = Quaternion.LookRotation(lookPos);
        
        if(!alreadyAttacked && timePassed > attackDelay)
        {
            Attack();

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
            timePassed = 0f;
        }
    }

    protected override void Attack()
    {
        SoundManager.instance.PlaySound(SoundManager.Sound.LightingAttack);
        RaycastHit hit;
        Vector3 direction = playerPos - transform.position;
        if(Physics.Raycast(transform.position, direction, out hit, range))
        {
            Debug.DrawRay(transform.position, (direction*10), Color.red, 10);

            //spawn attack particles
            lightingAttackEffect.transform.LookAt(playerPos);
            lightingAttackEffect.Play();

            EnemyBase enemy = hit.transform.GetComponentInParent<EnemyBase>();
            if(enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            Target target = hit.transform.GetComponent<Target>();
            if(target != null)
            {
                target.TakeDamage(damage);
            }

            PlayerMovement player = hit.transform.GetComponent<PlayerMovement>();
            if(player != null)
            {
                player.TakeDamage(damage);
            }

            if (hit.rigidbody != null){
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }

            Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
        }
    }
}
