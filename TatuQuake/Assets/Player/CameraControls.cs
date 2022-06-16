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

    public float mouseSensitivity = 100f;

    public Transform playerBody;

    private void Awake() 
    {
        playerInput = GetComponent<PlayerInput>();

        mouseInputX = playerInput.actions["LookX"];
        mouseInputY = playerInput.actions["LookY"];
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = mouseInputX.ReadValue<float>() * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseInputY.ReadValue<float>() * mouseSensitivity * Time.deltaTime;

        playerBody.Rotate(Vector3.up * mouseX);
    }
}
