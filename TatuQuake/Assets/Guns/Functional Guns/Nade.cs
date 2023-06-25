using UnityEngine;

public class Nade : MonoBehaviour
{
    private Rigidbody rigidBody;
    [SerializeField] private float nadeSpeed = 20f;
    [SerializeField] private float fuseTime = 1f;
    [SerializeField] private float blastRadius = 5f;
    [SerializeField] protected GameObject explosion;

    private float damage;
    private float impactForce;
    private Vector3 forward;
    private float timer;
    private bool isNew = true;

    private void Awake() 
    {
        rigidBody = GetComponent<Rigidbody>();
        NadeLauncher.OnFired += NadeFired;
        //layer 0 is the default layer (which is the layer is the nade is on)
        //layer 7 is the player
        Physics.IgnoreLayerCollision(0, 7, true);

    }

    private void Update() 
    {
        timer += Time.deltaTime;
        if(timer >= fuseTime){
            Explode();
        }
    }

    private void OnCollisionEnter(Collision other) 
    {
        //only exploding if we touch something that is *not* the world
        //world (or ground) layer is 6, 12 is NavMeshGround
        if(other.gameObject.layer != 6)
        {
            if(other.gameObject.layer != 12)
                Explode();
            else
                SoundManager.instance.PlaySound(SoundManager.Sound.NadeBounce, transform.position);
        }

        else
            SoundManager.instance.PlaySound(SoundManager.Sound.NadeBounce, transform.position);
    }

    private void NadeFired() 
    {
        if(isActiveAndEnabled)
        {
            //Only add force to a nade that hasn't already been fired out of the
            //launcher
            if(isNew == true)
            {
                //angle the nade up a bit
                Vector3 angle = new Vector3(0, 0.125f, 0);
                forward += angle;
                rigidBody.AddForce(forward * nadeSpeed, ForceMode.VelocityChange);
                isNew = false; //once a nade has been fired, it's no longer new
            }
        }
    }

    public void Explode()
    {
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
                float distance = Vector3.Distance(transform.position, player.transform.position) - 1;
                distance = Mathf.Clamp(distance, 0, blastRadius);
                float percentage = 1 - (distance/blastRadius);
                percentage = Mathf.Clamp(percentage, 0, 1);
                Vector3 direction = player.transform.position - (rigidBody.transform.position);
                //NadeLauncher shouldn't be as useful for movement as the rocket launcher is
                player.ApplyForce((impactForce * 0.6f), direction, percentage);
                player.TakeDamage((int)(damage / 1.9f));
            }
        }

        NadeLauncher.OnFired -= NadeFired;
        Destroy(gameObject.transform.GetChild(0).gameObject, 0.5f);
        transform.DetachChildren();
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
