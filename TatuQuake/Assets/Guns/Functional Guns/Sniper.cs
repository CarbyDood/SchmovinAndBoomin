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

    // Start is called before the first frame update
    void Start()
    {
        damage = 75f;
        range = 500f;
        fireRate = 2f;
        impactForce = 250f;
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

        if(zoom.triggered)
        {
            zoomed = !zoomed;
            animator.SetBool("Zoomed", zoomed);
            if(zoomed)
                StartCoroutine(Zoom());
            else
                UnZoom();
        }
    }

    //Use or get out of scope
    IEnumerator Zoom()
    {
        yield return new WaitForSeconds(0.15f);

        crossHair.SetActive(false);
        scopeReticle.SetActive(true);
        gunCam.SetActive(false);
        
        fpsCam.fieldOfView = zoomFOV;
    }

    private void UnZoom()
    {
        crossHair.SetActive(true);
        scopeReticle.SetActive(false);
        gunCam.SetActive(true);

        fpsCam.fieldOfView = ogFOV;
    }

    private void OnDisable() 
    {
        UnZoom();
        zoomed = false;
    }
}
