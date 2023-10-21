using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AmmoPickUps : MonoBehaviour
{
    private PlayerMovement player;
    private GameManager gameManager;
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

    private bool isTouchingPlayer = false;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
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

        //Allow the player to stay on an ammo pick up and pick is up instantly as soon as they start shooting their weapon
        if(isTouchingPlayer)
        {
            if(ammoType == "pistol")
            {
                if(pistolRef.currentAmmo < pistolRef.GetMaxAmmo())
                {
                    SoundManager.instance.PlaySound(SoundManager.Sound.AmmoPickUp);
                    pistolRef.IncreaseAmmo(ref pistolRef.currentAmmo, amount, pistolRef.GetMaxAmmo());
                    gameManager.ConsoleMessage("You found "+amount+ " pistol bullets");
                    if(canRespawn) gameManager.DisableObjectForTime(gameObject, 5);
                    else Destroy(gameObject);
                }
            }
            else if(ammoType == "sniper")
            {
                if(sniperRef.currentAmmo < sniperRef.GetMaxAmmo())
                {
                    SoundManager.instance.PlaySound(SoundManager.Sound.AmmoPickUp);
                    sniperRef.IncreaseAmmo(ref sniperRef.currentAmmo, amount, sniperRef.GetMaxAmmo());
                    gameManager.ConsoleMessage("You found "+amount+ " sniper bullets");
                    if(canRespawn) gameManager.DisableObjectForTime(gameObject, 5);
                    else Destroy(gameObject);
                }
            }
            else if(ammoType == "auto")
            {
                if(gameManager.currAutoAmmo < gameManager.maxAutoAmmo)
                {
                    SoundManager.instance.PlaySound(SoundManager.Sound.AmmoPickUp);
                    autoRef.IncreaseAmmo(ref gameManager.currAutoAmmo, amount, gameManager.maxAutoAmmo);
                    gameManager.ConsoleMessage("You found "+amount+ " auto slugs");
                    if(canRespawn) gameManager.DisableObjectForTime(gameObject, 5);
                    else Destroy(gameObject);
                }
            }
            else if(ammoType == "shell")
            {
                if(gameManager.currShellAmmo < gameManager.maxShellAmmo)
                {
                    SoundManager.instance.PlaySound(SoundManager.Sound.AmmoPickUp);
                    shellRef.IncreaseAmmo(ref gameManager.currShellAmmo, amount, gameManager.maxShellAmmo);
                    gameManager.ConsoleMessage("You found a pocket full of shells! ("+amount+")");
                    if(canRespawn) gameManager.DisableObjectForTime(gameObject, 5);
                    else Destroy(gameObject);
                }
            }
            else if(ammoType == "explosive")
            {
                if(gameManager.currExplosiveAmmo < gameManager.maxExplosiveAmmo)
                {
                    SoundManager.instance.PlaySound(SoundManager.Sound.AmmoPickUp);
                    explosiveRef.IncreaseAmmo(ref gameManager.currExplosiveAmmo, amount, gameManager.maxExplosiveAmmo);
                    gameManager.ConsoleMessage("You found "+amount+" boomlas!");
                    if(canRespawn) gameManager.DisableObjectForTime(gameObject, 5);
                    else Destroy(gameObject);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Player"))
        {
            isTouchingPlayer = true;
        }    
    }

    private void OnTriggerExit(Collider other) 
    {
        if(other.CompareTag("Player"))
        {
            isTouchingPlayer = false;
        }    
    }
}
