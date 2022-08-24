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

    private Transform myTrans;
    private bool reloaded = true;
    
    // Start is called before the first frame update
    void Start()
    {
        damage = 100f;
        range = 10000f;
        fireRate = 0.5f;
        impactForce = 440f;
    }

    // Update is called once per frame
    void Update()
    {
        //Exit out of firing animation when we are able to fire
        if(Time.time >= nextTimeToFire && animator.GetBool("Fired") == true)
        {
            animator.SetBool("Fired", false);
        }

        //Reload Rocket
        if(Time.time >= nextTimeToFire && reloaded == false)
        {
            reloadRocket.SetActive(false);
            dummyRocket.SetActive(true);
            reloaded = true;
        }

        //Semi Auto
        if(fire.triggered && Time.time >= nextTimeToFire && reloaded == true)
        {
            nextTimeToFire = Time.time + 1f/fireRate;
            Shoot();
        }
    }

    private new void Shoot()
    {
        animator.SetBool("Fired", true);
        Vector3 dumPos = dummyRocket.transform.position;
        Quaternion dumRot = dummyRocket.transform.rotation;
        dummyRocket.SetActive(false);
        reloadRocket.SetActive(true);
        Rocket rocketG = Instantiate(rocket, dumPos, dumRot);
        rocketG.SetDmg(damage);
        rocketG.SetFrc(impactForce);
        rocketG.SetFrwd(fpsCam.transform.forward);//make the rocket go towards where we're looking at
        reloaded = false;
        OnFired();
    }
}
