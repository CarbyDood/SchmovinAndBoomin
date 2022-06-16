using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{   
    private PlayerInput playerInput;

    //Store the actual controls
    private InputAction jumpin;
    private InputAction movin;

    //Clean up stuff to prevent memory leaks!
    private void OnDisable() 
    {
        
    }

    private void Awake() 
    {
        playerInput = GetComponent<PlayerInput>();

        jumpin = playerInput.actions["Jump"];
        movin = playerInput.actions["Move"];
    }

    private void Start() 
    {
        
    }

    private void FixedUpdate() 
    {
        
    }

    private void Jump(InputAction.CallbackContext context) 
    {

    }
}
