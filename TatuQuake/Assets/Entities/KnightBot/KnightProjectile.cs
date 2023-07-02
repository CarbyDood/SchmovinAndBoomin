using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightProjectile : MonoBehaviour
{
    [SerializeField] private float projSpeed = 1f;

    private float damage = 0f;
    private float impactForce;
    private Vector3 forward;
    private float timer;

    void Awake() 
    {
        Physics.IgnoreLayerCollision(0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += forward * projSpeed;
    }

    private void OnTriggerEnter(Collider other) 
    {
        //Damage
        EnemyBase enemy = other.gameObject.GetComponentInParent<EnemyBase>();
        if(enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        Target target = other.gameObject.GetComponent<Target>();
        if(target != null)
        {
            target.TakeDamage(damage);
        }

        PlayerMovement player = other.gameObject.GetComponent<PlayerMovement>();
        if(player != null)
        {
            player.TakeDamage((int)(damage));
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
    public void SetFrwd(Vector3 frwd)
    {
        forward = frwd;
    }
    public float GetDmg()
    {
        return damage;
    }

    public float GetFrc()
    {
        return impactForce;
    }
    public Vector3 GetFrwd()
    {
        return forward;
    }
}
