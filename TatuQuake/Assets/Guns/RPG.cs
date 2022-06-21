using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPG : WeaponsBaseClass
{
    public delegate void FireAction();
    public static event FireAction OnFired;

    [SerializeField] Rocket rocket;
    [SerializeField] GameObject dummyRocket;

    private Transform myTrans;
    private bool reloaded = true;
    
    // Start is called before the first frame update
    void Start()
    {
        damage = 100f;
        range = 10000f;
        fireRate = 0.8f;
        impactForce = 640f;
    }

    // Update is called once per frame
    void Update()
    {
        //Reload Rocket
        if(Time.time >= nextTimeToFire && reloaded == false)
        {
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
        Vector3 dumPos = dummyRocket.transform.position;
        Quaternion dumRot = dummyRocket.transform.rotation;
        dummyRocket.SetActive(false);
        Rocket rocketG = Instantiate(rocket, dumPos, dumRot);
        rocketG.SetDmg(damage);
        rocketG.SetFrc(impactForce);
        reloaded = false;
        OnFired();
    }
}
