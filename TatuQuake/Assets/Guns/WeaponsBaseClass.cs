using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponsBaseClass : MonoBehaviour
{
    [SerializeField] private float damage = 10f;
    [SerializeField] private float range = 100f;
    [SerializeField] private float fireRate = 15f;
    [SerializeField] private float impactForce = 50f;

    [SerializeField] private Camera fpsCam;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject impactEffect;

    [SerializeField] private PlayerInput playerInput;
    private InputAction fire;
    private InputAction changeFireMode;

    private int fireMode = 0; //0 = semi, 1 = full
    private float nextTimeToFire = 0f;

    private void Awake() 
    {
        fire = playerInput.actions["Fire"];
        changeFireMode = playerInput.actions["ChangeFireMode"];
    }

    // Update is called once per frame
    private void Update()
    {
        //Full Auto
        if(fire.ReadValue<float>() == 1 && Time.time >= nextTimeToFire && fireMode == 1)
        {
            nextTimeToFire = Time.time + 1f/fireRate;
            Shoot();
        }

        //Semi Auto
        else if(fire.triggered && Time.time >= nextTimeToFire && fireMode == 0)
        {
            nextTimeToFire = Time.time + 1f/fireRate;
            Shoot();
        }

        //Change firemode
        if(changeFireMode.triggered)
        {
            if(fireMode == 0){fireMode = 1;}
            else{fireMode = 0;}
            Debug.Log("Fire mode changed to "+fireMode);
        }
    }

    private void Shoot()
    {
        muzzleFlash.Play();
        RaycastHit hit;
        if(Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Target target = hit.transform.GetComponent<Target>();
            if(target != null)
            {
                target.TakeDamage(damage);
            }

            if (hit.rigidbody != null){
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }

            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGO, 2f);
        }
    }
}
