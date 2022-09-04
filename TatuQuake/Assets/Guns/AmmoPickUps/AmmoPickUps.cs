using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AmmoPickUps : MonoBehaviour
{
    [SerializeField] private PlayerMovement player;
    [SerializeField] private GameManager gameManager;

    private PlayerInput playerInput;
    protected InputAction fire;

    private Pistol pistolRef;
    private Sniper sniperRef;
    private SMG autoRef;
    private Shotgun shellRef;
    private NadeLauncher explosiveRef;
    private float bobHeight = 0.1f;
    private float bobSpeed = 3f;
    private float ogPosY;
    private float yRot = 0f;
    [SerializeField] private string ammoType;
    [SerializeField] private int amount;
    [SerializeField] private bool canRespawn;

    // Start is called before the first frame update
    void Start()
    {
        playerInput = player.GetComponent<PlayerInput>();
        fire = playerInput.actions["Fire"];
        pistolRef = player.gunViewModels[0].GetComponent<Pistol>();
        sniperRef = player.gunViewModels[6].GetComponent<Sniper>();
        autoRef = player.gunViewModels[1].GetComponent<SMG>();
        shellRef = player.gunViewModels[2].GetComponent<Shotgun>();
        explosiveRef = player.gunViewModels[4].GetComponent<NadeLauncher>();
        ogPosY = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        //spin and bob up and down
        Vector3 pos = transform.position;
        float newY = Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(pos.x, ogPosY + newY, pos.z);
        yRot += 0.3f;
        transform.rotation = Quaternion.Euler(0, yRot, 0);
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.name == "Player")
        {
            if(ammoType == "pistol")
            {
                if(pistolRef.currentAmmo < pistolRef.GetMaxAmmo())
                {
                    pistolRef.IncreaseAmmo(ref pistolRef.currentAmmo, amount, pistolRef.GetMaxAmmo());
                    if(canRespawn) gameManager.DisableObjectForTime(gameObject, 5);
                    else Destroy(gameObject);
                }
            }
            else if(ammoType == "sniper")
            {
                if(sniperRef.currentAmmo < sniperRef.GetMaxAmmo())
                {
                    sniperRef.IncreaseAmmo(ref sniperRef.currentAmmo, amount, sniperRef.GetMaxAmmo());
                    if(canRespawn) gameManager.DisableObjectForTime(gameObject, 5);
                    else Destroy(gameObject);
                }
            }
            else if(ammoType == "auto")
            {
                if(gameManager.currAutoAmmo < gameManager.maxAutoAmmo)
                {
                    autoRef.IncreaseAmmo(ref gameManager.currAutoAmmo, amount, gameManager.maxAutoAmmo);
                    if(canRespawn) gameManager.DisableObjectForTime(gameObject, 5);
                    else Destroy(gameObject);
                }
            }
            else if(ammoType == "shell")
            {
                if(gameManager.currShellAmmo < gameManager.maxShellAmmo)
                {
                    shellRef.IncreaseAmmo(ref gameManager.currShellAmmo, amount, gameManager.maxShellAmmo);
                    if(canRespawn) gameManager.DisableObjectForTime(gameObject, 5);
                    else Destroy(gameObject);
                }
            }
            else if(ammoType == "explosive")
            {
                if(gameManager.currExplosiveAmmo < gameManager.maxExplosiveAmmo)
                {
                    explosiveRef.IncreaseAmmo(ref gameManager.currExplosiveAmmo, amount, gameManager.maxExplosiveAmmo);
                    if(canRespawn) gameManager.DisableObjectForTime(gameObject, 5);
                    else Destroy(gameObject);
                }
            }
        }    
    }

    private void OnTriggerStay(Collider other) 
    {
        if(fire.ReadValue<float>() == 1)
        {
            if(ammoType == "pistol")
            {
                pistolRef.IncreaseAmmo(ref pistolRef.currentAmmo, amount, pistolRef.GetMaxAmmo());
                if(canRespawn) gameManager.DisableObjectForTime(gameObject, 5);
                else Destroy(gameObject);
            }
            else if(ammoType == "sniper")
            {
                sniperRef.IncreaseAmmo(ref sniperRef.currentAmmo, amount, sniperRef.GetMaxAmmo());
                if(canRespawn) gameManager.DisableObjectForTime(gameObject, 5);
                else Destroy(gameObject);
            }
            else if(ammoType == "auto")
            {
                autoRef.IncreaseAmmo(ref gameManager.currAutoAmmo, amount, gameManager.maxAutoAmmo);
                if(canRespawn) gameManager.DisableObjectForTime(gameObject, 5);
                else Destroy(gameObject);
            }
            else if(ammoType == "shell")
            {
                shellRef.IncreaseAmmo(ref gameManager.currShellAmmo, amount, gameManager.maxShellAmmo);
                if(canRespawn) gameManager.DisableObjectForTime(gameObject, 5);
                else Destroy(gameObject);
            }
            else if(ammoType == "auto")
            {
                explosiveRef.IncreaseAmmo(ref gameManager.currExplosiveAmmo, amount, gameManager.maxExplosiveAmmo);
                if(canRespawn) gameManager.DisableObjectForTime(gameObject, 5);
                else Destroy(gameObject);
            }
        }
    }
}
