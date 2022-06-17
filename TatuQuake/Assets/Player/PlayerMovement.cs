using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{   
    private CharacterController controller;
    private PlayerInput playerInput;
    
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    //Store the actual controls
    private InputAction jumpin;
    private InputAction movin;

    public float speed = 6f;
    public float gravity = -9.81f;
    public float jumpHeight = 9f;
    public float slideSpeed = -3f;

    //slope stuff
    private float groundRayDistance = 1;
    private RaycastHit slopeHit;

    Vector3 velocity;
    Vector3 move;
    bool isGrounded;

    private void Awake() 
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        jumpin = playerInput.actions["Jump"];
        movin = playerInput.actions["Move"];
    }

    private void Update() 
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -4.5f;
        }

        Vector2 currInput = movin.ReadValue<Vector2>();
        float x = currInput.x;
        float z = currInput.y;

        move = transform.right * x + transform.forward * z;

        if(OnSteepSlope())
        {
            SteepSlopeMove();
        }

        controller.Move(move * speed * Time.deltaTime);

        if(jumpin.triggered && isGrounded)
        {
            velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
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
        float slopeSlideSpeed = speed + slideSpeed + Time.deltaTime;

        move = slopeDirection * -slopeSlideSpeed;
        velocity.y = velocity.y - slopeHit.point.y;
    }
}
