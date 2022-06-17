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

    float xRotation = 0f;

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
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = mouseInputX.ReadValue<float>() * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseInputY.ReadValue<float>() * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
