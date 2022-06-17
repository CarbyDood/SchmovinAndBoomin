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

    Vector3 velocity;
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

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        if(jumpin.triggered && isGrounded)
        {
            velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}
