using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerMovement : MonoBehaviour
{   
    [SerializeField] private MomentumManager momentum;
    private CharacterController controller;
    private PlayerInput playerInput;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Animator animator;

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

    [SerializeField] private float baseSpeed = 2f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float baseJumpHeight = 1.7f;
    private float speed;
    private float jumpHeight;
    private float slideSpeed;
    [SerializeField] private GameObject groundCheck;
    [SerializeField] private GameObject ceilingCheck;
    private float groundDistance;
    private float ceilingDistance;
    public LayerMask groundMask;

    private float momentumLossRate = 1f;
    private float velocityCoeff = 0.005f;

    [SerializeField] public List<GameObject> gunViewModels;
    [SerializeField] private List<GameObject> gunWorldModels;
    //0 = Pistol, 1 = SMG, 2 = Shotgun, 3 = LMG, 4 = Nade Launcher, 5 = Super Shotgun, 6 = Sniper, 
    //7 = Rocket Launcher
    private int currGun;

    //slope stuff
    private float groundRayDistance = 0.9f;
    private RaycastHit slopeHit;

    Vector3 velocity;
    Vector3 move;
    Vector2 currInputVect;
    Vector2 smoothInputVelocity;
    [SerializeField] private float smoothInputSpeed = .2f;
    private float jumpCoolDown = 0.5f;
    private float nextTimeToJump = 0f;

    private bool isGrounded;
    private bool isOnStair;
    private bool headBonked;
    private bool startedFalling = false;
    private bool isFalling = false;
    private bool isJumping = false;
    private bool isAffectedByForce = false;

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

        int len = gunViewModels.Count;
        for(int i = 0; i < len; i++)
        {
            if(gunViewModels[i].activeSelf)
                currGun = i; //Get Default weapon
        }

        //Enable all weapons to "awake" them so we can properly keep track of their ammo counts
        foreach(GameObject weapon in gunViewModels)
        {
            weapon.SetActive(true);
        }

        disableWeapons();
        gameManager.DisableGunIcons();
        gunViewModels[currGun].SetActive(true);
        gunWorldModels[currGun].SetActive(true);
        gameManager.gunIcons[currGun].SetActive(true);
        gameManager.AddWeapon(currGun);

        speed = baseSpeed;
        jumpHeight = baseJumpHeight;
        slideSpeed = 3f;
        groundDistance = controller.radius;
        ceilingDistance = groundDistance/2;
    }

    private void Update() 
    {
        //Check if we are touching the ground, the ceiling, or some stairs
        isOnStair = Physics.CheckSphere(groundCheck.transform.position, groundDistance, groundMask);
        isGrounded = controller.isGrounded;
        headBonked = Physics.CheckSphere(ceilingCheck.transform.position, ceilingDistance, groundMask);

        if(headBonked && velocity.y > 0)
        {
            velocity.y = 0f;
            ceilingCheck.SetActive(false);
        }

        if(isGrounded)
        {
            velocity.x = 0f;
            velocity.z = 0f;
            velocity.y = -speed;
            ceilingCheck.SetActive(true);
            isFalling = false;
            isJumping = false;
            isAffectedByForce = false;
        }

        if(!isGrounded && !isOnStair && !startedFalling && !isFalling && velocity.y <= 0f){
            startedFalling = true;
            isFalling = true;
        }

        if(startedFalling && !isJumping && !isOnStair){
            velocity.y = -2f;
            startedFalling = false;
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

        //calculate the speed bonus given from momentum
        //Cap speed bonus to 3x
        speed = baseSpeed + (baseSpeed * 2 * (momentum.GetMomentum()/100));

        //controls when affected by outside force (explosive jumping)
        if(isAffectedByForce)
        {
            animator.SetBool("Jumpin", true);
            velocity += (currInputVect.x * transform.right * velocityCoeff);
            velocity += (currInputVect.y * transform.forward * velocityCoeff);

            //reduce speed
            speed /= 5;
        }

        //Air controls
        else if(!isGrounded && !isOnStair)
        {
            animator.SetBool("Jumpin", true);
            //reduce speed
            speed /= 1.2f;
        }

        else
            animator.SetBool("Jumpin", false);

        if(OnSteepSlope())
        {
            SteepSlopeMove();
            velocity.y = -jumpHeight - 1;
        }

        controller.Move(move * speed * Time.deltaTime);
    }

    private void Jump()
    {
        jumpHeight = baseJumpHeight + (baseJumpHeight  * (momentum.GetMomentum()/100));

        if(jumpin.triggered && isGrounded && Time.time >= nextTimeToJump)
        {
            isJumping = true;
            velocity.y += -velocity.y + Mathf.Sqrt(jumpHeight * -2f * gravity);
            nextTimeToJump = Time.time + jumpCoolDown;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    private void changeWeapon()
    {
        //Change weapon as soon as player hits the button

        //0 = Pistol, 1 = SMG, 2 = Shotgun, 3 = LMG, 4 = Nade Launcher, 5 = Super Shotgun, 6 = Sniper, 
        //7 = Rocket Launcher
        
        if(slot1.triggered && gameManager.HasWeapon(0))
        {
            //Disable all weapons so we can easily activate whatever weapon we want
            disableWeapons();
            currGun = 0;
        }

        else if(slot2.triggered && gameManager.HasWeapon(1))
        {
            //Disable all weapons so we can easily activate whatever weapon we want
            disableWeapons();
            currGun = 1;
        }

        else if(slot3.triggered && gameManager.HasWeapon(2))
        {
            //Disable all weapons so we can easily activate whatever weapon we want
            disableWeapons();
            currGun = 2;
        }

        else if(slot4.triggered && gameManager.HasWeapon(3))
        {
            //Disable all weapons so we can easily activate whatever weapon we want
            disableWeapons();
            currGun = 3;
        }

        else if(slot5.triggered && gameManager.HasWeapon(4))
        {
            //Disable all weapons so we can easily activate whatever weapon we want
            disableWeapons();
            currGun = 4;
        }

        else if(slot6.triggered && gameManager.HasWeapon(5))
        {
            //Disable all weapons so we can easily activate whatever weapon we want
            disableWeapons();
            currGun = 5;
        }

        else if(slot7.triggered && gameManager.HasWeapon(6))
        {
            //Disable all weapons so we can easily activate whatever weapon we want
            disableWeapons();
            currGun = 6;
        }

        else if(slot8.triggered && gameManager.HasWeapon(7))
        {
            //Disable all weapons so we can easily activate whatever weapon we want
            disableWeapons();
            currGun = 7;
        }

        gunViewModels[currGun].SetActive(true);
        gunWorldModels[currGun].SetActive(true);
        gameManager.DisableGunIcons();
        gameManager.gunIcons[currGun].SetActive(true);
        
        animator.SetInteger("CurrWeapon", currGun);
    }

    private void ScrollWeapon()
    {
        //keep track of original currGun value if we fail to find next weapon
        int ogCurrGun = currGun;
        //scroll wheel value comes in as a either 120 for scroll up, or -120 for scroll down
        int changeIndex = (int)-scroll.ReadValue<float>()/120;
        currGun += changeIndex;

        if(currGun > 7)
            currGun = 0; //scroll back to the start of the guns list
        if(currGun < 0)
            currGun = 7; //scroll back to the end of the guns list

        //Check to see next available weapon
        //allow 1 loop around, any more and there are no more guns to swap to!
        int loops = 0;
        while(!gameManager.HasWeapon(currGun) && (currGun >= 0 && currGun < 8) && loops < 2)
        {
            currGun += changeIndex;
            if(currGun < 0)
            {
                loops++;
                currGun = 7;
            }

            else if(currGun > 7)
            {
                loops++;
                currGun = 0;
            }
        }

        if(currGun < 0 || currGun > 7)
        {
            currGun = ogCurrGun;
        }

        disableWeapons();
        gameManager.DisableGunIcons();
        gunViewModels[currGun].SetActive(true);
        gunWorldModels[currGun].SetActive(true);
        gameManager.gunIcons[currGun].SetActive(true);
        animator.SetInteger("CurrWeapon", currGun);
    }

    private void disableWeapons()
    {
        //Disable all weapons
        foreach(GameObject weapon in gunViewModels)
        {
            weapon.SetActive(false);
        }

        foreach(GameObject weapon in gunWorldModels)
        {
            weapon.SetActive(false);
        }
    }

    private bool OnSteepSlope()
    {
        if(!isGrounded && !isOnStair) {return false;}

        //send a ray underneath the player that checks for any slopes. If a slope is hit that is greater than the slope angle
        //they are able to climb, then return true, because we are on a steepslope
        int layerMask = 1 << 6; //layer 6 is the ground, so we only care about ground colliders
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, (controller.height / 2) + groundRayDistance, layerMask))
        {
            float slopeAngle = Vector3.Angle(slopeHit.normal, Vector3.up);
            if(slopeAngle > controller.slopeLimit) {return true;}
        }
        return false;
    }

    private void SteepSlopeMove()
    {
        Vector3 slopeDirection = Vector3.up - slopeHit.normal * Vector3.Dot(Vector3.up, slopeHit.normal);
        move = slopeDirection * -slideSpeed;
        move.y = move.y - slopeHit.point.y;
    }

    public void ApplyForce(float force, Vector3 direction, float percentage)
    {
        isAffectedByForce = true;
        isJumping = true;
        float height = (force * percentage) / 100;
        velocity += direction * (2.5f * percentage);
        velocity.y += Mathf.Sqrt(height * -2f * gravity);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    public void SpeedLimit()
    {
        velocity.x = Mathf.Clamp(velocity.x, -8f, 8f);
        velocity.y = Mathf.Clamp(velocity.y, -8f, 8f);
        velocity.z = Mathf.Clamp(velocity.z, -8f, 8f);
    }

    public List<GameObject> GetGuns()
    {
        return gunViewModels;
    }

    public int GetCurrGun()
    {
        return currGun;
    }
}
