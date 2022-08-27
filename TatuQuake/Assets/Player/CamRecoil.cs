using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CamRecoil : MonoBehaviour
{
    //Rotations
    private Vector3 currRot;
    private Vector3 targRot;

    //Extra stuff
    private float smoothness;
    private float recenterSpeed;
    private float initialCamRotX;

    protected InputAction fire;
    protected InputAction mouseInputY;
    [SerializeField] protected PlayerInput playerInput;
    [SerializeField] private GameObject mainCam;

    void Awake() 
    {
        fire = playerInput.actions["Fire"];
        mouseInputY = playerInput.actions["LookY"];
    }

    void Update()
    {   
        //constantly tries to recenter camera
        targRot = Vector3.Lerp(targRot, Vector3.zero, recenterSpeed * Time.deltaTime);

        //move the camera slowly to the target spot
        currRot = Vector3.Slerp(currRot, targRot, smoothness * Time.fixedDeltaTime);

        transform.localRotation = Quaternion.Euler(currRot);
    }

    public void Recoil(float recoilX, float recoilY, float recoilZ, float smooth, float centerSpeed)
    {
        float camRotx = mainCam.transform.localRotation.x;
        float currRotx = transform.localRotation.x;
        float sum = camRotx + currRotx;

        if(Mathf.Abs(sum) > 0.70)
            targRot += new Vector3(0, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
        else
            targRot += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
        smoothness = smooth;
        recenterSpeed = centerSpeed;
    }
}
