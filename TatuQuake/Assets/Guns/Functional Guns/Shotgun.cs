using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : WeaponsBaseClass
{
    [SerializeField] private int pelletCount = 8;
    // Start is called before the first frame update
    void Start()
    {
        damage = 8f; //per pellet!
        range = 50f;
        fireRate = 1.5f;
        impactForce = 25f;
        recoilX = -15f;
        recoilY = 4f;
        recoilZ = 1f;
        smoothness = 6.5f;
        recenterSpeed = 2f;
    }

    // Update is called once per frame
    void Update()
    {
        //Exit out of firing animation when we are able to fire
        if(Time.time >= nextTimeToFire && animator.GetBool("Fired") == true)
        {
            animator.SetBool("Fired", false);
            worldAnimator.SetBool("Fired",false);
        }

        //Semi Auto
        if(fire.triggered && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f/fireRate;
            Shoot();
        }
    }

    private new void Shoot()
    {
        //shoot in a random spread
        //first pellet always hits dead center
        muzzleFlash.Play();
        worldMuzzleFlash.Play();
        animator.SetBool("Fired",true);
        worldAnimator.SetBool("Fired",true);

        recoilScript.Recoil(recoilX, recoilY, recoilZ, smoothness, recenterSpeed);
        for(int i = 0; i < pelletCount; i++)
        {
            RaycastHit hit;
            Vector3 rando;
            float spreadRange = 0.05f;
            if(i == 0)
            {
                rando = new Vector3(0, 0 ,0);
            }

            else{
                rando.x = Random.Range(-spreadRange, spreadRange);
                rando.y = Random.Range(-spreadRange, spreadRange);
                rando.z = Random.Range(-spreadRange, spreadRange);
            }

            if(Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward + rando, out hit, range))
            {
                //spawn bullet trail
                TrailRenderer worldViewTrail = Instantiate(worldTrail, worldMuzzleFlash.transform.position, Quaternion.identity);
                TrailRenderer trail = Instantiate(bulletTrail, muzzleFlash.transform.position, Quaternion.identity);
                StartCoroutine(SpawnTrail(worldViewTrail, hit.point));
                StartCoroutine(SpawnTrail(trail, hit.point));

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

            //if we hit nothing, show a trail towards the camera's center at a distance of the weapon's range
            else
            {
                TrailRenderer worldViewTrail = Instantiate(worldTrail, worldMuzzleFlash.transform.position, Quaternion.identity);
                TrailRenderer trail = Instantiate(bulletTrail, muzzleFlash.transform.position, Quaternion.identity);
                Vector3 pointTo = fpsCam.transform.position + ((fpsCam.transform.forward + rando) * range);
                StartCoroutine(SpawnTrail(worldViewTrail, pointTo));
                StartCoroutine(SpawnTrail(trail, pointTo));
            }
        }
    }
}
