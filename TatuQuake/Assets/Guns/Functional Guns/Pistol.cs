using UnityEngine;

public class Pistol : WeaponsBaseClass
{
    // Start is called before the first frame update
    void Start()
    {
        damage = 12f;
        range = 100f;
        fireRate = 7f;
        impactForce = 50f;
    }

    // Update is called once per frame
    void Update()
    {
        //Exit out of firing animation when we are able to fire
        if(Time.time >= nextTimeToFire && animator.GetBool("Fired") == true)
        {
            animator.SetBool("Fired", false);
        }

        //Semi Auto
        if(fire.triggered && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f/fireRate;
            Shoot();
        }
    }
}
