using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Sniper : WeaponsBaseClass
{
    [SerializeField] private GameObject scopeReticle;
    [SerializeField] private GameObject crossHair;
    [SerializeField] private GameObject gunCam;
    [SerializeField] private float zoomFOV = 15;
    [SerializeField] private float ogFOV = 110;
    private bool zoomed = false;
    private float zoomedRecoilX;
    private float zoomedRecoilY;
    private float zoomedRecoilZ;
    private float zoomedSmoothness;
    private float zoomedRecenterSpeed;
    [SerializeField] private CameraControls camCon;
    new void Awake()
    {
        fire = playerInput.actions["Fire"];
        changeFireMode = playerInput.actions["ChangeFireMode"];
        zoom = playerInput.actions["Zoom"];
        damage = 75f;
        range = 500f;
        fireRate = 0.9f;
        impactForce = 250f;
        recoilX = -32f;
        recoilY = 9f;
        recoilZ = 3.2f;
        smoothness = 12f;
        recenterSpeed = 1f;
        zoomedRecoilX = -20f;
        zoomedRecoilY = 6f;
        zoomedRecoilZ = 1.8f;
        zoomedSmoothness = 15f;
        zoomedRecenterSpeed = 3.5f;
        maxAmmo = 50;
        currentAmmo = 5;
        gunType = "sniper";
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

        if(zoom.triggered)
        {
            zoomed = !zoomed;
            animator.SetBool("Zoomed", zoomed);
            worldAnimator.SetBool("Zoomed", zoomed);

            if(zoomed)
                StartCoroutine(Zoom());
            else
                UnZoom();
        }
    }

    protected new void Shoot()
    {
        muzzleFlash.Play();
        worldMuzzleFlash.Play();
        animator.SetBool("Fired",true);
        worldAnimator.SetBool("Fired",true);
        RaycastHit hit;

        //Bitshift the weapon layer (8) to get a bit mask
        int layermask = 1 << 8;

        //we want to note down every collision *except* layer 8, so we use ~
        layermask = ~layermask;

        if(Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range, layermask))
        {
            //spawn bullet trail
            TrailRenderer worldViewTrail = Instantiate(worldTrail, worldMuzzleFlash.transform.position, Quaternion.identity);
            TrailRenderer trail = Instantiate(bulletTrail, muzzleFlash.transform.position, Quaternion.identity);
            StartCoroutine(SpawnTrail(worldViewTrail, hit.point));
            StartCoroutine(SpawnTrail(trail, hit.point));

            Target target = hit.transform.GetComponent<Target>();
            if(target != null)
            {
                target.TakeDamage(damage);
            }

            if (hit.rigidbody != null){
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }

            Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
        }

        //if we hit nothing, show a trail towards the camera's center at a distance of the weapon's range
        else
        {
            TrailRenderer worldViewTrail = Instantiate(worldTrail, worldMuzzleFlash.transform.position, Quaternion.identity);
            TrailRenderer trail = Instantiate(bulletTrail, muzzleFlash.transform.position, Quaternion.identity);
            Vector3 pointTo = fpsCam.transform.position + (fpsCam.transform.forward * range);
            StartCoroutine(SpawnTrail(worldViewTrail, pointTo));
            StartCoroutine(SpawnTrail(trail, pointTo));
        }

        if(zoomed)
            recoilScript.Recoil(zoomedRecoilX, zoomedRecoilY, zoomedRecoilZ, zoomedSmoothness, zoomedRecenterSpeed);
        else
            recoilScript.Recoil(recoilX, recoilY, recoilZ, smoothness, recenterSpeed);

        DecreaseAmmo(ref currentAmmo, 1);
    }

    //Use or get out of scope
    IEnumerator Zoom()
    {
        yield return new WaitForSeconds(0.15f);

        crossHair.SetActive(false);
        scopeReticle.SetActive(true);
        gunCam.SetActive(false);

        fpsCam.fieldOfView = zoomFOV;
        camCon.isZoomed = true;
    }

    private void UnZoom()
    {
        crossHair.SetActive(true);
        scopeReticle.SetActive(false);
        gunCam.SetActive(true);

        fpsCam.fieldOfView = ogFOV;
        camCon.isZoomed = false;
    }

    private void OnDisable() 
    {
        UnZoom();
        zoomed = false;
        worldAnimator.SetBool("Zoomed", false);
        animator.WriteDefaultValues();
        worldAnimator.WriteDefaultValues();
    }
}
