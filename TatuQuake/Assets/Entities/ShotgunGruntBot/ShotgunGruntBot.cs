using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunGruntBot : EnemyBase
{
    [SerializeField] float impactForce = 30f;
    private float range = 40f;
    [SerializeField] protected TrailRenderer entityTrail;
    [SerializeField] protected GameObject impactEffect;

    private new void Update() 
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
        SoundManager.instance.PlaySound(SoundManager.Sound.ShotgunShot);
        int pelletCount = 8;
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

            if(Physics.Raycast(transform.position, transform.forward + rando, out hit, range))
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

                GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactGO, 2f);
            }

            //if we hit nothing, show a trail towards the camera's center at a distance of the weapon's range
            else
            {
                TrailRenderer entityViewTrail = Instantiate(entityTrail, transform.position, Quaternion.identity);
                Vector3 pointTo = transform.position + (transform.forward * range);
                StartCoroutine(SpawnTrail(entityViewTrail, pointTo));
            }
        }
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