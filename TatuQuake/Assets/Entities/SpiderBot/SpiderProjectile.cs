using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpiderProjectile : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform player;
    protected Vector3 playerPos;
    private float impactForce;
    private float damage;
    [SerializeField] private float lifeTime;
    [SerializeField] protected GameObject explosion;
    [SerializeField] private float blastRadius = 1f;
    private float timePassed;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = player.GetComponent<PlayerMovement>().GetAimLocation();
        agent.SetDestination(playerPos);

        //explode after lifeTime amount
        if(timePassed >= lifeTime)
        {
            Explode();
        }

        timePassed += Time.deltaTime;
    }

    private void OnCollisionEnter(Collision other)
    {
        //Layer 7 is the player
        if(other.gameObject.layer == 7)
        {
            Explode();
        }
    }

    public void Explode()
    {
        //Layer 7 is the player
        Physics.IgnoreLayerCollision(0, 7, false);
        //Show the boomla
        GameObject boom = Instantiate(explosion, transform.position, transform.rotation);

        //Make boom sound
        SoundManager.instance.PlaySound(SoundManager.Sound.Explosion, transform.position);

        //Get nearby objects
        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);

        foreach (Collider nearbyObj in colliders)
        {
            //ignore other nades
            Nade nade = nearbyObj.GetComponent<Nade>();
            if(nade == null)
            {
                //Add Force
                Rigidbody rb = nearbyObj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(impactForce, transform.position, blastRadius);
                }
            }

            //Damage
            EnemyBase enemy = nearbyObj.GetComponentInParent<EnemyBase>();
            if(enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            Target target = nearbyObj.GetComponent<Target>();
            if(target != null)
            {
                target.TakeDamage(damage);
            }

            //Nade Jump!!
            PlayerMovement player = nearbyObj.GetComponent<PlayerMovement>();
            if(player != null)
            {
                player.TakeDamage((int)(damage));
            }
        }
        Destroy(gameObject);
    }

    public void SetDmg(float dmg)
    {
        damage = dmg;
    }

    public void SetFrc(float frc)
    {
        impactForce = frc;
    }

    public float GetDmg()
    {
        return damage;
    }

    public float GetFrc()
    {
        return impactForce;
    }

    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, blastRadius);
    }
}
