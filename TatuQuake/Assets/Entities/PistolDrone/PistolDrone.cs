using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolDrone : EnemyBase
{
    private float bobHeight = 0.1f;
    private float bobSpeed = 3f;
    private float climbTime = 2f;
    private float timePassed = 0f;
    private float ogPosY;
    private float ogHeight;
    private float ogOffset;
    private float maxHeight;
    private float maxOffset;

    [SerializeField] float impactForce = 50f;
    private float range = 100f;

    [SerializeField] protected TrailRenderer entityTrail;
    [SerializeField] protected GameObject impactEffect;

    private new void Start() 
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

        hitBox.attachedRigidbody.isKinematic = true;
        ogPosY = transform.position.y;
        ogHeight = agent.height;
        ogOffset = agent.baseOffset;
        maxHeight = ogHeight + (player.GetComponent<CharacterController>().height * 2.1f);
        maxOffset = ogOffset + (player.GetComponent<CharacterController>().height * 2.1f);
    }

    private new void Update() 
    {
        aggroTime += Time.deltaTime;

        playerPos = player.GetComponent<PlayerMovement>().GetAimLocation();

        //hover up and down
        Vector3 pos = transform.position;
        float newY = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(pos.x, ogPosY + newY, pos.z);

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
            Vibin();
            LowerDown();
        }

        if((playerInSightRange && !playerInAttackRange && playerInLineOfSight) || aggroTime <= chaseTime && (!playerInAttackRange || !playerInLineOfSight))
        {
            if(playerInSightRange && playerInLineOfSight)
            {
                aggroTime = 0f;
            }
            Huntin();
            LowerDown();
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

    protected new void Killin()
    {
        agent.SetDestination(transform.position);
        //drone should slowly climb up to stay 1 height about the player
        if(timePassed < climbTime)
        {
            agent.height = Mathf.Lerp(ogHeight, maxHeight, timePassed/climbTime);
            agent.baseOffset = Mathf.Lerp(ogOffset, maxOffset, timePassed/climbTime);
            timePassed += Time.deltaTime;
        }
        else
        {
            agent.height = maxHeight;
            agent.baseOffset = maxOffset;
        }

        transform.LookAt(playerPos);
        
        if(!alreadyAttacked)
        {
            Attack();

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    protected override void Attack()
    {
        SoundManager.instance.PlaySound(SoundManager.Sound.PistolShot);
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, range))
        {
            //spawn bullet trail
            TrailRenderer entityViewTrail = Instantiate(entityTrail, transform.position, Quaternion.identity);
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

            Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
        }

        //if we hit nothing, show a trail towards the camera's center at a distance of the weapon's range
        else
        {
            TrailRenderer entityViewTrail = Instantiate(entityTrail, transform.position, Quaternion.identity);
            Vector3 pointTo = transform.position + (transform.forward * range);
            StartCoroutine(SpawnTrail(entityViewTrail, pointTo));
        }
    }

    private void LowerDown()
    {
        if(timePassed >= 0)
        {
            agent.height = Mathf.Lerp(ogHeight, maxHeight, timePassed/climbTime);
            agent.baseOffset = Mathf.Lerp(ogOffset, maxOffset, timePassed/climbTime);
            timePassed -= Time.deltaTime;
        }
        else
        {
            agent.height = ogHeight;
            agent.baseOffset = ogOffset;
        }
    }

    protected override void Die()
    {
        animator.SetBool("IsDead", true);
        Debug.Log("Enemy "+gameObject.name+" died!");
        agent.enabled = false;
        animator.avatar = null;
        animator.enabled = false;
        hitBox.attachedRigidbody.isKinematic = false;
        hitBox.attachedRigidbody.velocity = Vector3.zero;
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
