using UnityEngine;

public class Pistol : WeaponsBaseClass
{
    // Start is called before the first frame update
    void Start()
    {
        damage = 10f;
        range = 100f;
        fireRate = 8f;
        impactForce = 50f;
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
}
