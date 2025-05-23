using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperShotgun : WeaponsBaseClass
{
    [SerializeField] private int pelletCount = 16;

    void OnEnable() 
    {
        worldAnimator.SetInteger("CurrWeapon", 5);
        worldAnimator.SetBool("Fired", false);
    }

    // Start is called before the first frame update
    void Start()
    {
        damage = 10f; //per pellet!
        range = 30f;
        fireRate = 0.88f;
        impactForce = 35f;
        recoilX = -30f;
        recoilY = 8f;
        recoilZ = 2f;
        smoothness = 13f;
        recenterSpeed = 1.8f;
        maxAmmo = gameManager.maxShellAmmo;
        currentAmmo = gameManager.currShellAmmo;
        gunType = "shell";
    }

    // Update is called once per frame
    void Update()
    {
        currentAmmo = gameManager.currShellAmmo;
        //Exit out of firing animation when we are able to fire
        if(Time.time >= nextTimeToFire && animator.GetBool("Fired") == true)
        {
            animator.SetBool("Fired", false);
            worldAnimator.SetBool("Fired",false);
        }

        //Semi Auto
        if(fire.triggered && Time.time >= nextTimeToFire && currentAmmo > 0)
        {
            nextTimeToFire = Time.time + 1f/fireRate;
            SoundManager.instance.PlaySound(SoundManager.Sound.SuperShottyShot);
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
            float spreadRange = 0.15f;
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

                EnemyBase enemy = hit.transform.GetComponentInParent<EnemyBase>();
                if(enemy != null)
                {
                    enemy.TakeDamage(damage * player.GetDamageMultiplier());
                }

                Target target = hit.transform.GetComponent<Target>();
                if(target != null)
                {
                    target.TakeDamage(damage * player.GetDamageMultiplier());
                }

                TargetDummy targetDummy = hit.transform.GetComponent<TargetDummy>();
                if(targetDummy != null)
                {
                    targetDummy.TurnOnRagdoll();
                }

                TargetTrigger targetTrig = hit.transform.GetComponent<TargetTrigger>();
                if(targetTrig != null)
                {
                    targetTrig.OnHit();
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
        currentAmmo = DecreaseAmmo(ref gameManager.currShellAmmo, 2);
    }

    public void PlayPumpBackAudio()
    {
        SoundManager.instance.PlaySound(SoundManager.Sound.SuperShottyPumpBack);
    }

    public void PlayPumpForwardAudio()
    {
        SoundManager.instance.PlaySound(SoundManager.Sound.SuperShottyPumpForward);
    }
}
