using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControls : MonoBehaviour
{
    private PlayerInput playerInput;

    //store mouse actions
    private InputAction mouseInputX;
    private InputAction mouseInputY;

    public float mouseSensitivity = 0.35f;

    public float zoomSens = 0.15f;
    public bool isZoomed = false;
    public Transform playerBody;

    public float xRotation = 0f;

    private void OnDisable()
    {
        mouseInputX.performed -= camUpdate;
        mouseInputX.canceled -= camUpdate;

        mouseInputY.performed -= camUpdate;
        mouseInputY.canceled -= camUpdate;
    }

    private void Awake() 
    {
        playerInput = GetComponent<PlayerInput>();

        mouseInputX = playerInput.actions["LookX"];
        mouseInputY = playerInput.actions["LookY"];
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        
        mouseInputX.performed += camUpdate;
        mouseInputX.canceled += camUpdate;
        
        mouseInputY.performed += camUpdate;
        mouseInputY.canceled += camUpdate;
    }

    private void camUpdate(InputAction.CallbackContext context) 
    {
        float mouseX, mouseY;
        if(isZoomed)
        {
            mouseX = mouseInputX.ReadValue<float>() * zoomSens;
            mouseY = mouseInputY.ReadValue<float>() * zoomSens;
        } 
        else
        {
            mouseX = mouseInputX.ReadValue<float>() * mouseSensitivity;
            mouseY = mouseInputY.ReadValue<float>() * mouseSensitivity;
        }

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
