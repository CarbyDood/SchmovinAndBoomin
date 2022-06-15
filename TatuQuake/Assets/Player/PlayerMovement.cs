using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{   
    private Rigidbody tatuRigidbody;
    private PlayerInput playerInput;
    private PlayerControls playerInputActions;
    private void Awake() 
    {
        tatuRigidbody = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        playerInputActions = new PlayerControls();
        playerInputActions.Movement.Enable();
        playerInputActions.Movement.Jump.performed += Jump;
        playerInputActions.Movement.Move.performed += Move_performed;
    }

    private void FixedUpdate() 
    {
        Vector2 inputVect = playerInputActions.Movement.Move.ReadValue<Vector2>();
        float speed = 2.75f;
        tatuRigidbody.AddForce(new Vector3(inputVect.x, 0, inputVect.y) * speed, ForceMode.Force);
    }
    private void Move_performed(InputAction.CallbackContext context)
    {
        Debug.Log(context);
        Vector2 inputVect = context.ReadValue<Vector2>();
        float speed = 5f;
        tatuRigidbody.AddForce(new Vector3(inputVect.x, 0, inputVect.y) * speed, ForceMode.Force);
    }

    public void Jump(InputAction.CallbackContext context) {
        Debug.Log(context);
        if (context.performed){
            Debug.Log("Go ahead jump, might as well jump! Context is: " + context.phase);
            tatuRigidbody.AddForce(Vector3.up * 4.75f, ForceMode.Impulse);
        }
    }
}
