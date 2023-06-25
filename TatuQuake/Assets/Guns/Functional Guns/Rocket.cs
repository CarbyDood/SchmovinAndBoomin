using UnityEngine;

public class Rocket : MonoBehaviour
{
    private Rigidbody rigidBody;
    [SerializeField] private float rocketSpeed = 15f;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private float blastRadius = 3f;
    [SerializeField] private bool isDummy = false;
    [SerializeField] protected ParticleSystem trial;
    [SerializeField] protected GameObject explosion;

    private float damage;
    private float impactForce;
    private Vector3 forward;
    private float timer;
    private float timeToReplayAudio = 1.683f;
    private float audioTime = 0f;

    private void Awake() 
    {
        rigidBody = GetComponent<Rigidbody>();
        RPG.OnFired += MissleFired;
        //layer 0 is the default layer (which is the layer is the rocket is on)
        //layer 7 is the player
        Physics.IgnoreLayerCollision(0, 7, true);
        SoundManager.instance.PlaySoundAsChild(SoundManager.Sound.Rocket, gameObject);
        audioTime = Time.time + timeToReplayAudio;
    }

    private void Update() 
    {
        timer += Time.deltaTime;
        if(timer >= lifeTime && !isDummy)
            Explode();

        if(Time.time >= audioTime)
        {
            SoundManager.instance.PlaySoundAsChild(SoundManager.Sound.Rocket, gameObject);
            audioTime = Time.time + timeToReplayAudio;
        }
    }

    private void OnCollisionEnter(Collision other) 
    {
        Explode();
    }

    private void MissleFired() 
    {
        if(isActiveAndEnabled)
        {
            trial.Play();
            rigidBody.AddForce(forward * rocketSpeed, ForceMode.Impulse);
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
            //Add Force
            Rigidbody rb = nearbyObj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(impactForce, transform.position, blastRadius);
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

            //Rocket Jump!!!
            PlayerMovement player = nearbyObj.GetComponent<PlayerMovement>();
            if(player != null)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position) - 1;
                distance = Mathf.Clamp(distance, 0, blastRadius);
                float percentage = 1 - (distance/blastRadius);
                percentage = Mathf.Clamp(percentage, 0, 1);
                Vector3 direction = player.transform.position - (rigidBody.transform.position);
                player.ApplyForce(impactForce, direction, percentage);
                player.TakeDamage((int)(damage / 1.9f));
            }
        }

        RPG.OnFired -= MissleFired;
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
