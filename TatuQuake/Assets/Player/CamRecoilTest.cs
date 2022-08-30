using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CamRecoilTest : MonoBehaviour
{
    //Rotations
    private Vector3 currRot;
    private Vector3 targRot;

    //Extra stuff
    private float smoothness;
    private float recenterSpeed;
    private float initialCamRotX;
    private Quaternion startMainCamRot;
    private float startXRot;

    protected InputAction fire;
    protected InputAction mouseInputY;
    [SerializeField] protected PlayerInput playerInput;
    [SerializeField] private Transform mainCam;
    [SerializeField] private CameraControls camCon;

    void Awake() 
    {
        fire = playerInput.actions["Fire"];
        mouseInputY = playerInput.actions["LookY"];
    }

    void Update()
    {   
        float mouseY = mouseInputY.ReadValue<float>() * camCon.mouseSensitivity;

        if(fire.ReadValue<float>() == 1)
        {
            //pulling up
            if(mouseY > 0 && mainCam.transform.rotation.x < startMainCamRot.x)
            {
                startMainCamRot = mainCam.transform.localRotation;
                startXRot = camCon.xRotation;
            }

            //if we pull down futher than where we started, punish the player for overcompensating by having them look further down
            //than their starting aimpoint
            if(mainCam.transform.rotation.x > startMainCamRot.x)
            {
                startMainCamRot = mainCam.transform.localRotation;
                startXRot = camCon.xRotation;
            }
        }

        if(fire.GetButtonDown())
        {
            startMainCamRot = mainCam.transform.localRotation;
            startXRot = camCon.xRotation;
        }

        if(fire.GetButtonUp())
        {
            mainCam.transform.localRotation = startMainCamRot;
            camCon.xRotation = startXRot;
            targRot = startMainCamRot.eulerAngles;

            startMainCamRot = new Quaternion(0f, 0f, 0f, 0f);
            startXRot = 0f;
        }

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

        if(Mathf.Abs(sum) > 0.80)
            targRot += new Vector3(0, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
        else
            targRot += new Vector3(recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
        smoothness = smooth;
        recenterSpeed = centerSpeed;
    }
}
