using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponsBaseClass : MonoBehaviour
{
    [SerializeField] protected float damage;
    [SerializeField] protected float range;
    [SerializeField] protected float fireRate;
    [SerializeField] protected float impactForce;

    [SerializeField] protected Camera fpsCam;
    [SerializeField] protected ParticleSystem muzzleFlash;
    [SerializeField] protected GameObject impactEffect;
    [SerializeField] protected MomentumManager momentum;

    [SerializeField] protected PlayerInput playerInput;
    protected InputAction fire;
    protected InputAction changeFireMode;

    protected int fireMode = 0; //0 = semi, 1 = full
    protected float nextTimeToFire = 0f;

    protected void Awake() 
    {
        fire = playerInput.actions["Fire"];
        changeFireMode = playerInput.actions["ChangeFireMode"];
    }

    // Update is called once per frame
    private void Update()
    {
        /*//Change firemode
        if(changeFireMode.triggered)
        {
            if(fireMode == 0){fireMode = 1;}
            else{fireMode = 0;}
            Debug.Log("Fire mode changed to "+fireMode);
        }*/
    }

    protected void Shoot()
    {
        muzzleFlash.Play();
        RaycastHit hit;
        if(Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Target target = hit.transform.GetComponent<Target>();
            if(target != null)
            {
                target.TakeDamage(damage);
            }

            if (hit.rigidbody != null){
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }

            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGO, 2f);
        }
    }

    public float GetDamage()
    {
        return damage;
    }

    public float GetRange()
    {
        return range;
    }

    public float GetFireRate()
    {
        return fireRate;
    }

    public float GetImpactForce()
    {
        return impactForce;
    }

    public void SetDamage(float dmg)
    {
        damage = dmg;
    }

    public void SetRange(float rng)
    {
        range = rng;
    }

    public void SetFireRate(float firrat)
    {
        fireRate = firrat;
    }

    public void SetImapactForce(float frc)
    {
        impactForce = frc;
    }
}
