using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SniperBot : EnemyBase
{
    [SerializeField] float impactForce = 15f;
    private float range = 100f;
    [SerializeField] protected TrailRenderer entityTrail;
    [SerializeField] protected GameObject impactEffect;
    [SerializeField] protected float attackDelay = 1f;
    [SerializeField] protected Transform shotOrigin;
    [SerializeField] private LineRenderer laserSight;
    private float timePassed = 0f;
    private bool trackPlayer = true;

    private new void Awake()
    {
        aggroTime = chaseTime;
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        laserSight.enabled = false;
        laserSight.SetPosition(0, shotOrigin.position);
        SetRagdollParts();
    }



    private new void Update() 
    {
        //Animation stuff
        if(agent.velocity.magnitude >= 0.1f)
            animator.SetBool("IsMoving",true);
        else
            animator.SetBool("IsMoving",false);

        if(trackPlayer)
            playerPos = player.GetComponent<PlayerMovement>().GetAimLocation();
        laserSight.SetPosition(0, shotOrigin.position);

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
            animator.SetBool("IsAiming", false);
            animator.SetBool("IsAttacking", false);
            laserSight.enabled = false;
            trackPlayer = true;
            timePassed = 0f;
            Vibin();
        }

        if((playerInSightRange && !playerInAttackRange && playerInLineOfSight) || aggroTime <= chaseTime && (!playerInAttackRange || !playerInLineOfSight))
        {
            animator.SetBool("IsAiming", false);
            animator.SetBool("IsAttacking", false);
            laserSight.enabled = false;
            trackPlayer = true;
            timePassed = 0f;
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
        timePassed += Time.deltaTime;
    }

    private new void Killin()
    {
        if(timePassed < attackDelay)
        {
            animator.SetBool("IsAiming", true);
        }

        laserSight.SetPosition(1, playerPos);
        laserSight.enabled = true;
        //look at player but not on the y axis
        agent.SetDestination(transform.position);
        Vector3 lookPos = playerPos - transform.position;
        lookPos.y = 0;
        transform.rotation = Quaternion.LookRotation(lookPos);
        
        //stop tracking a split second before firing to allow player the chance to dodge attack!
        if(!alreadyAttacked && timePassed >= (attackDelay - 0.1f))
        {
            trackPlayer = false;
        }

        if(!alreadyAttacked && timePassed >= attackDelay)
        {
            animator.SetBool("IsAiming", false);
            animator.SetBool("IsAttacking", true);
            Attack();

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
            Invoke(nameof(ResetTimer), timeBetweenAttacks);
            trackPlayer = true;
        }
    }

    protected override void Attack()
    {
        SoundManager.instance.PlaySound(SoundManager.Sound.SniperShot);

        Vector3 direction = playerPos - shotOrigin.position;
        RaycastHit hit;
        if(Physics.Raycast(shotOrigin.position, direction, out hit, range))
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
            Vector3 pointTo = shotOrigin.position + (transform.forward * range);
            StartCoroutine(SpawnTrail(entityViewTrail, pointTo));
        }
    }

    private void ResetTimer()
    {
        animator.SetBool("IsAttacking", false);
        timePassed = 0f;
    }

    protected override void Die()
    {
        animator.SetBool("IsDead", true);
        laserSight.enabled = false;
        Debug.Log("Enemy "+gameObject.name+" died!");
        agent.enabled = false;
        TurnOnRagdoll();
        Destroy(gameObject, 5f);
        gameManager.EnemyKilled();
        this.enabled = false;
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