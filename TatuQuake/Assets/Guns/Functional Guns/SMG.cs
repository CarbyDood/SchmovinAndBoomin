using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMG : WeaponsBaseClass
{
    // Start is called before the first frame update
    void Start()
    {
        damage = 7f;
        range = 70f;
        fireRate = 17f;
        impactForce = 30f;
    }

    // Update is called once per frame
    void Update()
    {
        //Full Auto
        if(fire.ReadValue<float>() == 1 && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f/fireRate;
            Shoot();
        }
    }
}
