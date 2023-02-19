using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<bool> gunInventory;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private GameObject playerObj;
    [SerializeField] private TextMeshProUGUI ammoCount;
    [SerializeField] private TextMeshProUGUI health;
    [SerializeField] private TextMeshProUGUI armour;
    [SerializeField] private GameObject statuses;
    [SerializeField] public List<GameObject> gunIcons;
    [SerializeField] public GameObject HUD;
    [SerializeField] public GameObject ScoreBoard;
    [SerializeField] private Camera playerFpCam;
    [SerializeField] public Transform spawnPoint;


    //shared ammo pools
    public int maxAutoAmmo = 200;
    public int maxShellAmmo = 50;
    public int maxExplosiveAmmo = 25;
    public int currAutoAmmo = 20;
    public int currShellAmmo = 5;
    public int currExplosiveAmmo = 2;

    //enum for health/power up statuses
    public enum Status
    {
        Healthy,
        Hurt,
        Wounded,
        Critical,
        SuperShellActive,
        MaxMomentumActive,
        TatuPowerActive
    }

    void Awake()
    {
        //Add 8 falses to gunInventory
        for(int i = 0; i < 8; i++)
        {
            gunInventory.Add(false);
        }

        //debug stuff, take out when shipping!!!
        //GiveAllWeapons();
    }

    // Update is called once per frame
    void Update()
    {
        //Update UI elements
        int ammo;
        if(player.GetCurrGun() == 7)
            //Launcher script is on a child of rocketLauncher object
            ammo = player.GetGuns()[player.GetCurrGun()].transform.GetChild(1).GetComponent<WeaponsBaseClass>().GetCurrAmmo();
        else
            ammo = player.GetGuns()[player.GetCurrGun()].GetComponent<WeaponsBaseClass>().GetCurrAmmo();

        ammoCount.text = ("Ammo: "+ammo).ToString();
        health.text = ("Health: "+player.GetHealth()+"%").ToString();
        armour.text = ("Armour: "+player.GetArmour()).ToString();

    }

    private void GiveAllWeapons()
    {
        for(int i = 0; i < gunInventory.Count; i++)
        {
            gunInventory[i] = true;
        }
    }

    private void TakeAllWeapons()
    {
        for(int i = 0; i < gunInventory.Count; i++)
        {
            gunInventory[i] = false;
        }
    }

    public void AddWeapon(int index)
    {
        if(index < gunInventory.Count && index >= 0)
            gunInventory[index] = true;
    }
    
    public bool HasWeapon(int index)
    {
        if(index >= gunInventory.Count || index < 0)
            return false;
        else
            return gunInventory[index];
    }

    public void DisableGunIcons()
    {
        //Disable all gunIcons
        foreach(GameObject icon in gunIcons)
        {
            icon.SetActive(false);
        }
    }

    public void DisableHud()
    {
        HUD.SetActive(false);
    }

    public void EnableHud()
    {
        HUD.SetActive(true);
    }

    public void ShowScores()
    {
        foreach(Transform child in HUD.transform)
            child.gameObject.SetActive(false);
        
        ScoreBoard.SetActive(true);
    }

    //method to update Health/power up icon
    public int UpdateStatus(Status stat)
    {
        if(stat == Status.Healthy)
        {
            DisableAllStatusIcon();
            statuses.transform.GetChild(0).gameObject.SetActive(true);
            return 0;
        }

        else if(stat == Status.Hurt)
        {
            DisableAllStatusIcon();
            statuses.transform.GetChild(1).gameObject.SetActive(true);
            return 1;
        }

        else if(stat == Status.Wounded)
        {
            DisableAllStatusIcon();
            statuses.transform.GetChild(2).gameObject.SetActive(true);
            return 2;
        }

        else if(stat == Status.Critical)
        {
            DisableAllStatusIcon();
            statuses.transform.GetChild(3).gameObject.SetActive(true);
            return 3;
        }

        else if(stat == Status.SuperShellActive)
        {
            DisableAllStatusIcon();
            statuses.transform.GetChild(4).gameObject.SetActive(true);
            return 4;
        }

        else if(stat == Status.MaxMomentumActive)
        {
            DisableAllStatusIcon();
            statuses.transform.GetChild(5).gameObject.SetActive(true);
            return 5;
        }

        else if(stat == Status.TatuPowerActive)
        {
            DisableAllStatusIcon();
            statuses.transform.GetChild(6).gameObject.SetActive(true);
            return 6;
        }

        return -1;
    }

    //helper method to disable all status icons
    public void DisableAllStatusIcon()
    {
        foreach(Transform child in statuses.transform)
            child.gameObject.SetActive(false);
    }

    public void DisableObjectForTime(GameObject obj, float time)
    {
        StartCoroutine(DisableObject(obj, time));
    }

    public IEnumerator DisableObject(GameObject obj, float time)
    {
        obj.SetActive(false);
        yield return new WaitForSeconds(time);
        obj.SetActive(true);
    }

}
