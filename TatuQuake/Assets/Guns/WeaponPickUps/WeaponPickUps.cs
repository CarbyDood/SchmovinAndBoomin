using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickUps : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private PlayerMovement player;
    private float bobHeight = 0.1f;
    private float bobSpeed = 3f;
    private float ogPosY;
    private float yRot = 0f;
    [SerializeField] private int gunItGives;
    //0 = Pistol, 1 = SMG, 2 = Shotgun, 3 = LMG, 4 = Nade Launcher, 5 = Super Shotgun, 6 = Sniper, 
    //7 = Rocket Launcher
    [SerializeField] private bool canDisappear;
    [SerializeField] private bool inMP;
    private WeaponsBaseClass weapRef;
    private Pistol pistolRef;
    private Sniper sniperRef;
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        ogPosY = transform.position.y;
        pistolRef = player.gunViewModels[0].GetComponent<Pistol>();
        sniperRef = player.gunViewModels[6].GetComponent<Sniper>();
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
        if(!gameManager.HasWeapon(gunItGives))
        {
            SoundManager.instance.PlaySound(SoundManager.Sound.WeaponPickUp);
            gameManager.AddWeapon(gunItGives);
            
            //console messages
            if(gunItGives == 0) gameManager.ConsoleMessage("You picked up the Pistol");
            if(gunItGives == 1) gameManager.ConsoleMessage("You picked up the SMG");
            if(gunItGives == 2) gameManager.ConsoleMessage("You picked up the Shotty");
            if(gunItGives == 3) gameManager.ConsoleMessage("You picked up the LMG");
            if(gunItGives == 4) gameManager.ConsoleMessage("You picked up the Nade Launcher");
            if(gunItGives == 5) gameManager.ConsoleMessage("You picked up the Super Shotty");
            if(gunItGives == 6) gameManager.ConsoleMessage("You picked up the Sniper");
            if(gunItGives == 7) gameManager.ConsoleMessage("You picked up the Rocket Launcher!");

            if(gunItGives == 0) pistolRef.IncreaseAmmo(ref pistolRef.currentAmmo, 20, pistolRef.GetMaxAmmo());
            if(gunItGives == 6) pistolRef.IncreaseAmmo(ref sniperRef.currentAmmo, 3, sniperRef.GetMaxAmmo());
            if(gunItGives == 1 || gunItGives == 3) pistolRef.IncreaseAmmo(ref gameManager.currAutoAmmo, 20, gameManager.maxAutoAmmo);
            if(gunItGives == 2 || gunItGives == 5) pistolRef.IncreaseAmmo(ref gameManager.currShellAmmo, 10, gameManager.maxShellAmmo);
            if(gunItGives == 4 || gunItGives == 7) pistolRef.IncreaseAmmo(ref gameManager.currExplosiveAmmo, 2, gameManager.maxExplosiveAmmo);
            if(canDisappear) Destroy(gameObject);
        }
    }
}
