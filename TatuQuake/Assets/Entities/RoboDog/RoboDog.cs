using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboDog : EnemyBase
{
    //[SerializeField] float impactForce = 30f;
    [SerializeField] private GameObject attackSphere;
    [SerializeField] private float attackArea = 1f;
    private float timePassed = 0f;
    private bool jumped = false;
    private bool jumping = false;
    private float jumpTime = 0.8f;
    private Vector3 startPos;
    private Vector3 jumpPos;
    private bool playerInBiteRange;

    private new void Update() 
    { 
        playerPos = player.GetComponent<PlayerMovement>().GetAimLocation();
        //Check for sight and attack ranges
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, playerMask);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerMask);
        playerInBiteRange = Physics.CheckSphere(attackSphere.transform.position, attackArea, playerMask);

        if(jumping == false)
        {
            if(!playerInSightRange && !playerInAttackRange)
            {
                animator.SetBool("WindingUp",false);
                animator.SetBool("IsRunning",false);
                animator.SetBool("IsWalking",true);
                jumped = false;
                timePassed = 0f;
                Vibin();
            }

            if(playerInSightRange && !playerInAttackRange)
            {
                animator.SetBool("WindingUp",false);
                animator.SetBool("IsWalking",false);
                animator.SetBool("IsRunning",true);
                jumped = false;
                timePassed = 0f;
                Huntin();
            }
        }

        if(playerInSightRange && playerInAttackRange)
        {
            Killin();
        }
    }

    private new void Killin()
    {
        //Dog should do a small "charge up" and then jump at the players position
        if(jumped == false && jumping == false)
        {
            animator.SetBool("IsWalking",false);
            animator.SetBool("IsRunning",false);
            animator.SetBool("WindingUp",true);
            //look at player but not on the y axis, and only when not jumping!
            Vector3 lookPos = playerPos - transform.position;
            lookPos.y = 0;
            transform.rotation = Quaternion.LookRotation(lookPos);

            agent.SetDestination(transform.position);
            if(timePassed > 0.8f)
            {
                animator.SetBool("WindingUp",false);
                jumping = true;
                startPos = transform.position;
                jumpPos = playerPos;
                jumpPos.y = startPos.y;
                timePassed = 0f;
                animator.SetBool("Jumping",true);
            }
        }

        if(jumping == true)
        {
            agent.enabled = false;
            transform.position = Vector3.Lerp(startPos, jumpPos, (timePassed*0.95f)/jumpTime);
        }

        if(timePassed > jumpTime)
        {
            jumped = true;
            agent.enabled = true;
            jumping = false;
            animator.SetBool("Jumping",false);

            //Should stay still to attack player, otherwise if outside of bite range, chase the player
            if(playerInBiteRange)
            {
                Debug.Log("TurnOff");
                agent.SetDestination(transform.position);
                animator.SetBool("IsRunning",false);
            }
            else
            {
                Debug.Log("wat");
                agent.SetDestination(playerPos);
                animator.SetBool("IsRunning",true);
            }
        }

        timePassed += Time.deltaTime;

        if(!alreadyAttacked && playerInBiteRange)
        {
            animator.SetBool("IsRunning", false);
            animator.SetBool("Attacked", true);
            Attack();

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    protected override void Attack()
    {

        Collider[] colliders = Physics.OverlapSphere(attackSphere.transform.position, attackArea);
        foreach (Collider nearbyObj in colliders)
        {
            PlayerMovement player = nearbyObj.GetComponent<PlayerMovement>();
            if(player != null)
            {
                player.TakeDamage(damage);
            }
        }
    }

    protected new void ResetAttack()
    {
        animator.SetBool("Attacked", false);
        alreadyAttacked = false;
    }

    private IEnumerator WaitFor(float secs)
    {
        Debug.Log("Waitin");
        yield return new WaitForSeconds(secs);
    }

    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(attackSphere.transform.position, attackArea);
    }
}
