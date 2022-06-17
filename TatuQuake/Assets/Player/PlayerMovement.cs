using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{   
    private CharacterController controller;
    private PlayerInput playerInput;

    //Store the actual controls
    private InputAction jumpin;
    private InputAction movin;

    public float speed = 12f;

    //Clean up stuff to prevent memory leaks!
    private void OnDisable() 
    {
        
    }

    private void Awake() 
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        jumpin = playerInput.actions["Jump"];
        movin = playerInput.actions["Move"];
    }

    private void Start() 
    {
        
    }

    private void Update() 
    {
        Vector2 currInput = movin.ReadValue<Vector2>();
        float x = currInput.x;
        float z = currInput.y;

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);
    }

    private void Jump(InputAction.CallbackContext context) 
    {

    }
}
