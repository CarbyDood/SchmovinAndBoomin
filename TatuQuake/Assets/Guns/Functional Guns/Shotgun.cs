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
        fireRate = 2f;
        impactForce = 25f;
    }

    // Update is called once per frame
    void Update()
    {
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

                GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactGO, 2f);
            }

            //if we hit nothing, show a trail towards the camera's center at a distance of the weapon's range
            else
            {
                TrailRenderer trail = Instantiate(bulletTrail, muzzleFlash.transform.position, Quaternion.identity);
                Vector3 pointTo = fpsCam.transform.position + ((fpsCam.transform.forward + rando) * range);
                StartCoroutine(SpawnTrail(trail, pointTo));
            }
        }
    }
}
