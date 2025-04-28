using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using TMPro;

public class GameManager : MonoBehaviour
{
    private PlayerInput playerInput;
    [SerializeField] private List<bool> gunInventory;
    [SerializeField] private List<Message> msgList = new List<Message>();
    private int maxMsgs = 8;
    private Message currHintMsg; 
    [SerializeField] private LevelData currLevelStats;
    [SerializeField] GameObject consolePanel, hintPanel, textObj, hintTextObj;
    [SerializeField] private PlayerMovement player;
    [SerializeField] private GameObject playerObj;
    [SerializeField] private TextMeshProUGUI ammoCount;
    [SerializeField] private TextMeshProUGUI health;
    [SerializeField] private TextMeshProUGUI armour;
    [SerializeField] private GameObject statuses;
    [SerializeField] public List<GameObject> gunIcons;
    [SerializeField] public GameObject HUD;
    [SerializeField] public GameObject ScoreBoard;
    [SerializeField] public GameObject DeathMessage;
    [SerializeField] public GameObject VictoryMessage;
    private InputAction showScoreboard;
    [SerializeField] private Camera playerFpCam;
    [SerializeField] public Transform spawnPoint;
    private bool countTime = true;

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

        playerInput = GetComponent<PlayerInput>();
        showScoreboard = playerInput.actions["ScoreBoard"]; 

        //debug stuff, take out when shipping!!!
        //GiveAllWeapons();
    }

    // Update is called once per frame
    void Update()
    {
        //Keep track of time
        if(countTime)
            currLevelStats.time = Time.time;

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

        ScoreBoard.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = currLevelStats.levelName;
        ScoreBoard.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"Time: \t{(int)TimeSpan.FromSeconds(currLevelStats.time).TotalMinutes}:{TimeSpan.FromSeconds(currLevelStats.time).Seconds:00}";
        ScoreBoard.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = $"Enemies:\t{currLevelStats.enemiesKilled}/{currLevelStats.enemies}";
        ScoreBoard.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = $"Secrets:\t{currLevelStats.secretsFound}/{currLevelStats.secrets}";
        if(currLevelStats.fenFound){ScoreBoard.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = "Hidden Fen: [x]";}
        if(currLevelStats.statueFound){ScoreBoard.transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = "Statue: [x]";}

        //See if player is hitting button to make showboard show up
        if(showScoreboard.triggered && !player.isDed)
        {
            ShowScores();
        }
        else if(showScoreboard.WasReleasedThisFrame() && !player.isDed)
        {
            HideScores();
        }

        if(player.isDed)
        {
            ShowDeathMessage();
        }
        if(player.isWon)
        {
            ShowVictoryMessage();
        }

        //Make sure msgList is empty
        if(msgList.Count > 0)
        {
            if(msgList[0].textObj == null)
                msgList.Remove(msgList[0]);
        }
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
        {
            gunInventory[index] = true;
        }
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
        {
            if(child.gameObject.name != "ScopeReticle")
                child.gameObject.SetActive(false);
        }

        ScoreBoard.SetActive(true);
    }

    public void HideScores()
    {
        foreach(Transform child in HUD.transform)
        {
            if(child.gameObject.name != "ScopeReticle" && child.gameObject.name != "Respawn Text" && child.gameObject.name != "Victory Text" && child.gameObject.name != "ContinueText")
            {
                child.gameObject.SetActive(true);
            }
        }
        ScoreBoard.SetActive(false);
    }

    public void ShowDeathMessage()
    {
        DeathMessage.SetActive(true);
    }

    public void ShowVictoryMessage()
    {
        VictoryMessage.SetActive(true);
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

    //Console Messages to alert the player of things like what pick up they picked up
    public void ConsoleMessage(string msg)
    {
        //List should not hold more than 8 messages at any time
        if(msgList.Count >= maxMsgs)
        {
            Destroy(msgList[0].textObj.gameObject);
            msgList.Remove(msgList[0]);
        }

        //Pop up a text box in the top right that stays for 5 seconds
        Message daMsg = new Message();
        daMsg.text = msg;
        GameObject newText = Instantiate(textObj, consolePanel.transform);
        daMsg.textObj = newText.GetComponent<TextMeshProUGUI>();
        daMsg.textObj.text = daMsg.text;
        msgList.Add(daMsg);

        Destroy(msgList.Last().textObj.gameObject, 4);
    }

    //Hint Message to guide player on various game mechanics
    public void HintMessage(string hint_msg, float secToDelete)
    {
        if(currHintMsg != null && currHintMsg.textObj != null)
        {
            Destroy(currHintMsg.textObj.gameObject);
        }
        //Pop up a text box in the top right that stays for 5 seconds
        Message daMsg = new Message();
        daMsg.text = hint_msg;
        GameObject newText = Instantiate(hintTextObj, hintPanel.transform);
        daMsg.textObj = newText.GetComponent<TextMeshProUGUI>();
        daMsg.textObj.text = daMsg.text;
        daMsg.textObj.fontSize = 32f;
        currHintMsg = daMsg;

        Destroy(currHintMsg.textObj.gameObject, secToDelete);
    }

    public void SecretFound()
    {
        currLevelStats.secretsFound += 1;
    }

    public void EnemyKilled()
    {
        currLevelStats.enemiesKilled += 1;
    }

    public void FenFound()
    {
        currLevelStats.fenFound = true;
    }

    public void StatueFound()
    {
        currLevelStats.statueFound = true;
    }

    public void SetCountTime(bool status)
    {
        countTime = status;
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

[System.Serializable]
public class Message
{
    public TextMeshProUGUI textObj;
    public string text;
    public float timeAdded;
}