using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NadeLauncher : WeaponsBaseClass
{
    public delegate void FireAction();
    public static event FireAction OnFired;

    [SerializeField] Nade nade;
    [SerializeField] GameObject startPosition;

    private Transform myTrans;
    
    // Start is called before the first frame update
    void Start()
    {
        damage = 80f;
        range = 10000f;
        fireRate = 0.8f;
        impactForce = 450f;
        recoilX = -10f;
        recoilY = 5f;
        recoilZ = 1f;
        smoothness = 8f;
        recenterSpeed = 1f;
        maxAmmo = gameManager.maxExplosiveAmmo;
        currentAmmo = gameManager.currExplosiveAmmo;
        gunType = "explosive";
    }

    // Update is called once per frame
    void Update()
    {
        currentAmmo = gameManager.currExplosiveAmmo;
        //Exit out of firing animation when we are able to fire
        if(Time.time >= nextTimeToFire && animator.GetBool("Fired") == true)
        {
            animator.SetBool("Fired", false);
            worldAnimator.SetBool("Fired", false);
        }

        //Semi Auto
        if(fire.triggered && Time.time >= nextTimeToFire && currentAmmo > 0)
        {
            nextTimeToFire = Time.time + 1f/fireRate;
            Shoot();
        }
    }

    private new void Shoot()
    {
        animator.SetBool("Fired", true);
        worldAnimator.SetBool("Fired", true);
        recoilScript.Recoil(recoilX, recoilY, recoilZ, smoothness, recenterSpeed);
        Vector3 nadeStartPos = startPosition.transform.position;
        Quaternion nadeStartRot = startPosition.transform.rotation;
        Nade projectile = Instantiate(nade, fpsCam.transform.position, nadeStartRot);
        projectile.SetDmg(damage);
        projectile.SetFrc(impactForce);
        projectile.SetFrwd(fpsCam.transform.forward);
        currentAmmo = DecreaseAmmo(ref gameManager.currExplosiveAmmo, 1);
        OnFired();
    }
}
