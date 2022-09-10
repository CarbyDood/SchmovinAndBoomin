using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMG : WeaponsBaseClass
{
    void OnEnable() 
    {
        worldAnimator.SetInteger("CurrWeapon", 1);
        worldAnimator.SetBool("Fired", false);
    }
    // Start is called before the first frame update
    void Start()
    {
        damage = 7f;
        range = 70f;
        fireRate = 17f;
        impactForce = 30f;
        recoilX = -4f;
        recoilY = 3f;
        recoilZ = 0.9f;
        smoothness = 7f;
        recenterSpeed = 2.8f;
        maxAmmo = gameManager.maxAutoAmmo;
        currentAmmo = gameManager.currAutoAmmo;
        gunType = "auto";
    }

    // Update is called once per frame
    void Update()
    {
        currentAmmo = gameManager.currAutoAmmo;
        //Stop fire animations when we're out of ammo
        if(currentAmmo <= 0) 
        {
            animator.SetBool("Fired",false);
            worldAnimator.SetBool("Fired",false);
        }

        //Exit out of firing animation when we let go of the fire key
        if(fire.ReadValue<float>() != 1 && animator.GetBool("Fired") == true)
        {
            animator.SetBool("Fired", false);
            worldAnimator.SetBool("Fired",false);
        }

        //Full Auto
        if(fire.ReadValue<float>() == 1 && Time.time >= nextTimeToFire && currentAmmo > 0)
        {
            nextTimeToFire = Time.time + 1f/fireRate;
            SoundManager.instance.PlaySound(SoundManager.Sound.SMGShot);
            Shoot();
        }
    }
}
