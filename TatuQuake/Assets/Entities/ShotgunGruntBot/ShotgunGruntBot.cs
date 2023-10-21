using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunGruntBot : EnemyBase
{
    [SerializeField] float impactForce = 30f;
    private float range = 40f;
    [SerializeField] protected TrailRenderer entityTrail;
    [SerializeField] protected GameObject impactEffect;
    [SerializeField] private Transform shotOriginL, shotOriginR;
    private bool altShot = false;

    private new void Update() 
    {
        //Animation stuff
        if(agent.velocity.magnitude >= 0.1f)
            animator.SetBool("IsMoving",true);
        else
            animator.SetBool("IsMoving",false);

        aggroTime += Time.deltaTime;

        playerPos = player.GetComponent<PlayerMovement>().GetAimLocation();

        //Check for sight and attack ranges
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerMask);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);
        
        //Line of sight
        if(playerInSightRange)
        {
            RaycastHit hit;

            if(Physics.Raycast(sightOrigin.position, playerPos - sightOrigin.position, out hit,  Mathf.Infinity, ~entityMask))
            {
                playerInLineOfSight = hit.collider.CompareTag("Player");
            }
        }

        if(((!playerInSightRange && !playerInAttackRange) || !playerInLineOfSight) && aggroTime >= chaseTime)
        {
            animator.SetBool("IsAttacking", false);
            Vibin();
        }

        if((playerInSightRange && !playerInAttackRange && playerInLineOfSight) || aggroTime <= chaseTime && (!playerInAttackRange || !playerInLineOfSight))
        {
            animator.SetBool("IsAttacking", false);
            if(playerInSightRange && playerInLineOfSight)
            {
                aggroTime = 0f;
            }
            Huntin();
        }

        if(playerInSightRange && playerInAttackRange && playerInLineOfSight)
        {
            if(playerInSightRange && playerInLineOfSight)
            {
                aggroTime = 0f;
            }
            Killin();
        }
    }

    private new void Killin()
    {
        animator.SetBool("IsAttacking", true);
        //look at player but not on the y axis
        agent.SetDestination(transform.position);
        Vector3 lookPos = playerPos - transform.position;
        lookPos.y = 0;
        transform.rotation = Quaternion.LookRotation(lookPos);
        
        if(!alreadyAttacked)
        {
            Attack();

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    protected override void Attack()
    {
        Transform shotOrigin;
        if(altShot){shotOrigin = shotOriginL;}
        else{shotOrigin = shotOriginR;}

        SoundManager.instance.PlaySound(SoundManager.Sound.ShotgunShot);
        int pelletCount = 8;
        Vector3 direction = playerPos - shotOrigin.position;
        for(int i = 0; i < pelletCount; i++)
        {
            RaycastHit hit;
            Vector3 rando;
            float spreadRange = 0.25f;
            if(i == 0)
            {
                rando = new Vector3(0, 0 ,0);
            }

            else{
                rando.x = Random.Range(-spreadRange, spreadRange);
                rando.y = Random.Range(-spreadRange, spreadRange);
                rando.z = Random.Range(-spreadRange, spreadRange);
            }

            if(Physics.Raycast(shotOrigin.position, direction + rando, out hit, range))
            {
                //spawn bullet trail
                TrailRenderer entityViewTrail = Instantiate(entityTrail, shotOrigin.position, Quaternion.identity);
                StartCoroutine(SpawnTrail(entityViewTrail, hit.point));

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

                GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactGO, 2f);
            }

            //if we hit nothing, show a trail towards the camera's center at a distance of the weapon's range
            else
            {
                TrailRenderer entityViewTrail = Instantiate(entityTrail, shotOrigin.position, Quaternion.identity);
                Vector3 pointTo = shotOrigin.position + (shotOrigin.forward * range);
                StartCoroutine(SpawnTrail(entityViewTrail, pointTo));
            }
        }

        altShot = !altShot;
    }

    public IEnumerator SpawnTrail(TrailRenderer trail, Vector3 dest)
    {
        float time = 0;
        Vector3 startPos = trail.transform.position;

        while (time < 0.5)
        {
            trail.transform.position = Vector3.Lerp(startPos, dest, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }
        trail.transform.position = dest;

        Destroy(trail.gameObject, trail.time);
    }
}