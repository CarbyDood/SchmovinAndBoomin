using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{   
    [SerializeField] private MomentumManager momentum;
    private CharacterController controller;
    private PlayerInput playerInput;
    [SerializeField] private Animator animator;
    
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    //Store the actual controls
    private InputAction jumpin;
    private InputAction movin;
    private InputAction slot1;
    private InputAction slot2;
    private InputAction slot3;
    private InputAction slot4;
    private InputAction slot5;
    private InputAction slot6;
    private InputAction slot7;
    private InputAction slot8;
    private InputAction scroll;

    [SerializeField] private float baseSpeed = 4f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float baseJumpHeight = 4f;
    [SerializeField] private float baseSlideSpeed = -3f;
    private float speed;
    private float jumpHeight;
    private float slideSpeed;
    private float momentumLossRate = 1f;
    private float velocityCoeff = 0.005f;

    [SerializeField] private List<GameObject> guns;
    //0 = Pistol, 1 = SMG, 2 = Shotgun, 3 = LMG, 4 = Nade Launcher, 4 = Super Shotgun, 5 = Sniper, 
    //6 = Rocket Launcher
    private int currGun;

    //slope stuff
    private float groundRayDistance = 1;
    private RaycastHit slopeHit;

    Vector3 velocity;
    Vector3 move;
    Vector2 currInputVect;
    Vector2 smoothInputVelocity;
    [SerializeField] private float smoothInputSpeed = .2f;

    bool isGrounded;

    private void Awake() 
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        jumpin = playerInput.actions["Jump"];
        movin = playerInput.actions["Move"];

        slot1 = playerInput.actions["WeaponSlot1"];
        slot2 = playerInput.actions["WeaponSlot2"];
        slot3 = playerInput.actions["WeaponSlot3"];
        slot4 = playerInput.actions["WeaponSlot4"];
        slot5 = playerInput.actions["WeaponSlot5"];
        slot6 = playerInput.actions["WeaponSlot6"];
        slot7 = playerInput.actions["WeaponSlot7"];
        slot8 = playerInput.actions["WeaponSlot8"];
        scroll = playerInput.actions["ScrollWeapon"];

        int len = guns.Count;
        for(int i = 0; i < len; i++)
        {
            if(guns[i].activeSelf)
                currGun = i; //Get Default weapon
        }

        speed = baseSpeed;
        jumpHeight = baseJumpHeight;
        slideSpeed = baseSlideSpeed;
    }

    private void Update() 
    {
        //Check if we are touching the ground
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if(isGrounded && velocity.y < 0)
        {
            velocity.x = 0f;
            velocity.z = 0f;
            velocity.y = -4.5f;
        }

        SpeedLimit();
        Move();
        Jump();
        changeWeapon();

        if(scroll.triggered){
            ScrollWeapon();
        }

        velocity.y += gravity * Time.deltaTime;

        //reset position if we fall off the map
        if(transform.position.y < -50)
        {
            controller.enabled = false;
            controller.transform.position = new Vector3(1, 2, -3);
            controller.enabled = true;
        }
    }

    private void Move()
    {
        Vector2 input = movin.ReadValue<Vector2>();
        currInputVect = Vector2.SmoothDamp(currInputVect, input, ref smoothInputVelocity, smoothInputSpeed);

        //if player is staying on the move, increase their momentum!
        if(Mathf.Abs(currInputVect.x) > 0.01 || Mathf.Abs(currInputVect.y) > 0.01)
        {
            animator.SetBool("Runnin", true);
            float absX = Mathf.Abs(currInputVect.x);
            float absY = Mathf.Abs(currInputVect.y);
            momentum.AddMomentum(Mathf.Max(absX, absY));
        }

        //if the player is staying still, decrease momentum
        else
        {
            animator.SetBool("Runnin", false);
            momentum.SubMomentum(momentumLossRate);
        }

        //move based off where we're rotated/looking
        move = transform.right * currInputVect.x + transform.forward * currInputVect.y;

        if(OnSteepSlope())
        {
            SteepSlopeMove();
        }

        //calculate the speed bonus given from momentum
        //and if we're in the air
        speed = baseSpeed + (momentum.GetMomentum()/10);

        //Air controls
        if(!isGrounded)
        {
            animator.SetBool("Jumpin", true);
            //Fight against velocity forces
            velocity += currInputVect.x * transform.right * velocityCoeff;
            velocity += currInputVect.y * transform.forward * velocityCoeff;
            controller.Move(velocity * Time.deltaTime);

            //reduce speed
            speed /= 5;
        }

        else
            animator.SetBool("Jumpin", false);
        
        controller.Move(move * speed * Time.deltaTime);
    }

    private void Jump()
    {
        jumpHeight = baseJumpHeight + ((momentum.GetMomentum()/10)/3f);
        if(jumpin.triggered && isGrounded)
        {
            velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        controller.Move(velocity * Time.deltaTime);
    }

    private void changeWeapon()
    {
        //Change weapon as soon as player hits the button

        //0 = Pistol, 1 = SMG, 2 = Shotgun, 3 = LMG, 4 = Nade Launcher, 5 = Super Shotgun, 6 = Sniper, 
        //7 = Rocket Launcher
        if(slot1.triggered)
        {
            //Disable all weapons so we can easily activate whatever weapon we want
            disableWeapons();
            currGun = 0;
            guns[currGun].SetActive(true);
        }

        else if(slot2.triggered)
        {
            //Disable all weapons so we can easily activate whatever weapon we want
            disableWeapons();
            currGun = 1;
            guns[currGun].SetActive(true);
        }

        else if(slot3.triggered)
        {
            //Disable all weapons so we can easily activate whatever weapon we want
            disableWeapons();
            currGun = 2;
            guns[currGun].SetActive(true);
        }

        else if(slot4.triggered)
        {
            //Disable all weapons so we can easily activate whatever weapon we want
            disableWeapons();
            currGun = 3;
            guns[currGun].SetActive(true);
        }

        else if(slot5.triggered)
        {
            //Disable all weapons so we can easily activate whatever weapon we want
            disableWeapons();
            currGun = 4;
            guns[currGun].SetActive(true);
        }

        else if(slot6.triggered)
        {
            //Disable all weapons so we can easily activate whatever weapon we want
            disableWeapons();
            currGun = 5;
            guns[currGun].SetActive(true);
        }

        else if(slot7.triggered)
        {
            //Disable all weapons so we can easily activate whatever weapon we want
            disableWeapons();
            currGun = 6;
            guns[currGun].SetActive(true);
        }

        else if(slot8.triggered)
        {
            //Disable all weapons so we can easily activate whatever weapon we want
            disableWeapons();
            currGun = 7;
            guns[currGun].SetActive(true);
        }
    }

    private void ScrollWeapon()
    {
        //scroll wheel value comes in as a either 120 for scroll up, or -120 for scroll down
        int changeIndex = (int)-scroll.ReadValue<float>()/120;
        currGun += changeIndex;

        if(currGun > 7)
            currGun = 0; //scroll back to the start of the guns list
        if(currGun < 0)
            currGun = 7; //scroll back to the end of the guns list

        disableWeapons();
        guns[currGun].SetActive(true);
    }

    private void disableWeapons()
    {
        //Disable all weapons
        foreach(GameObject weapon in guns)
        {
            weapon.SetActive(false);
        }
    }

    private bool OnSteepSlope()
    {
        if(!isGrounded) {return false;}

        //send a ray underneath the player that checks for any slopes. If a slope is hit that is greater than the slope angle
        //they are able to climb, then return true, because we are on a steepslope/
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, (controller.height / 2) + groundRayDistance))
        {
            float slopeAngle = Vector3.Angle(slopeHit.normal, Vector3.up);
            if(slopeAngle > controller.slopeLimit) {return true;}
        }
        return false;
    }

    private void SteepSlopeMove()
    {
        Vector3 slopeDirection = Vector3.up - slopeHit.normal * Vector3.Dot(Vector3.up, slopeHit.normal);
        slideSpeed = baseSlideSpeed - (momentum.GetMomentum()/10);
        float slopeSlideSpeed = speed + slideSpeed + Time.deltaTime;

        move = slopeDirection * -slopeSlideSpeed;
        move.y = move.y - slopeHit.point.y;
    }

    public void ApplyForce(float force, Vector3 direction)
    {
        float height = force/100;
        velocity += direction * 2.5f;
        velocity.y += Mathf.Sqrt(height * -2f * gravity);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void SpeedLimit(){
        if(velocity.x >= 8f) {velocity.x = 8f;}
        if(velocity.z >= 8f) {velocity.z = 8f;}
        if(velocity.y >= 8f) {velocity.y = 8f;}
    }
}
