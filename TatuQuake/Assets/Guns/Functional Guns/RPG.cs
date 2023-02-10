using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPG : WeaponsBaseClass
{
    public delegate void FireAction();
    public static event FireAction OnFired;

    [SerializeField] Rocket rocket;
    [SerializeField] GameObject dummyRocket;
    [SerializeField] GameObject reloadRocket;
    [SerializeField] GameObject worldRocket;
    

    private Transform myTrans;
    private bool reloaded = true;
    
    void OnEnable() 
    {
        worldAnimator.SetInteger("CurrWeapon", 7);
        worldAnimator.SetBool("Fired", false);
    }

    // Start is called before the first frame update
    void Start()
    {
        damage = 100f;
        range = 10000f;
        fireRate = 0.5f;
        impactForce = 440f;
        recoilX = -15f;
        recoilY = 7.5f;
        recoilZ = 1.5f;
        smoothness = 6f;
        recenterSpeed = 0.9f;
        maxAmmo = gameManager.maxExplosiveAmmo;
        currentAmmo = gameManager.currExplosiveAmmo;
        gunType = "explosive";
    }

    // Update is called once per frame
    void Update()
    {
        currentAmmo = gameManager.currExplosiveAmmo;
        //Exit out of firing animation when we are able to fire if we still have ammo
        if(Time.time >= nextTimeToFire && currentAmmo > 0)
        {
            animator.SetBool("Fired", false);
            worldAnimator.SetBool("Fired", false);
            animator.SetBool("LastShot",false);
        }

        //Reload Rocket
        if(Time.time >= nextTimeToFire && reloaded == false && currentAmmo > 0)
        {
            reloadRocket.SetActive(false);
            dummyRocket.SetActive(true);
            worldRocket.SetActive(true);
            reloaded = true;
        }

        //Semi Auto
        if(fire.triggered && Time.time >= nextTimeToFire && reloaded == true && currentAmmo > 0)
        {
            nextTimeToFire = Time.time + 1f/fireRate;
            SoundManager.instance.PlaySound(SoundManager.Sound.RPGShot);
            Shoot();
        }

        else if(currentAmmo <= 0)
        {
            animator.SetBool("LastShot", false);
            dummyRocket.SetActive(false);
            worldRocket.SetActive(false);
            reloaded = false;
        }
    }

    private new void Shoot()
    {
        currentAmmo = DecreaseAmmo(ref gameManager.currExplosiveAmmo, 1);

        if(currentAmmo > 0) 
        {
            animator.SetBool("Fired", true);
        }

        else if(currentAmmo <= 0)
        {
            animator.SetBool("LastShot",true);
        }
        worldAnimator.SetBool("Fired", true);
        recoilScript.Recoil(recoilX, recoilY, recoilZ, smoothness, recenterSpeed);
        Vector3 dumPos = dummyRocket.transform.position;
        Quaternion dumRot = dummyRocket.transform.rotation;
        dummyRocket.SetActive(false);
        reloadRocket.SetActive(true);
        worldRocket.SetActive(false);
        Rocket rocketG = Instantiate(rocket, fpsCam.transform.position, dumRot);
        rocketG.SetDmg(damage * player.GetDamageMultiplier());
        rocketG.SetFrc(impactForce);
        rocketG.SetFrwd(fpsCam.transform.forward);//make the rocket go towards where we're looking at
        reloaded = false;
        OnFired();
    }
}
