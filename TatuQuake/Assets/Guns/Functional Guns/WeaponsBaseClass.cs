using System.Collections;
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
    [SerializeField] protected TrailRenderer bulletTrail;
    [SerializeField] protected MomentumManager momentum;

    [SerializeField] protected PlayerInput playerInput;
    [SerializeField] protected Animator animator;
    protected InputAction fire;
    protected InputAction changeFireMode;
    protected InputAction zoom;

    protected int fireMode = 0; //0 = semi, 1 = full
    protected float nextTimeToFire = 0f;

    protected void Awake() 
    {
        fire = playerInput.actions["Fire"];
        changeFireMode = playerInput.actions["ChangeFireMode"];
        zoom = playerInput.actions["Zoom"];
    }

    protected void Shoot()
    {
        muzzleFlash.Play();
        animator.SetBool("Fired",true);
        RaycastHit hit;

        //Bitshift the weapon layer (8) to get a bit mask
        int layermask = 1 << 8;

        //we want to note down every collision *except* layer 8, so we use ~
        layermask = ~layermask;

        if(Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range, layermask))
        {
            //spawn bullet trail
            TrailRenderer trail = Instantiate(bulletTrail, muzzleFlash.transform.position, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, hit.point));

            Target target = hit.transform.GetComponent<Target>();
            if(target != null)
            {
                target.TakeDamage(damage);
            }

            if (hit.rigidbody != null){
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }

            Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
        }

        //if we hit nothing, show a trail towards the camera's center at a distance of the weapon's range
        else
        {
            TrailRenderer trail = Instantiate(bulletTrail, muzzleFlash.transform.position, Quaternion.identity);
            Vector3 pointTo = fpsCam.transform.position + (fpsCam.transform.forward * range);
            StartCoroutine(SpawnTrail(trail, pointTo));
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

    void OnDisable() 
    {
        animator.WriteDefaultValues();
    }
}
