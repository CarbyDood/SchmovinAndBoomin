using UnityEngine;

public class Pistol : WeaponsBaseClass
{
    new void Awake()
    {
        fire = playerInput.actions["Fire"];
        changeFireMode = playerInput.actions["ChangeFireMode"];
        zoom = playerInput.actions["Zoom"];
        damage = 12f;
        range = 100f;
        fireRate = 7f;
        impactForce = 50f;
        recoilX = -7f;
        recoilY = 3.2f;
        recoilZ = 0.3f;
        smoothness = 6.5f;
        recenterSpeed = 4f;
        maxAmmo = 120;
        currentAmmo = 100;
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
        if(fire.triggered && Time.time >= nextTimeToFire && currentAmmo > 0)
        {
            nextTimeToFire = Time.time + 1f/fireRate;
            Shoot();
        }
    }
}
