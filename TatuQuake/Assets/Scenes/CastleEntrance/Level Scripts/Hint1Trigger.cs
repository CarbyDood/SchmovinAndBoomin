using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hint1Trigger : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField] private int msgTime;
    [SerializeField] private string hintMsg;

    private void Start() 
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Player"))
        {
            SoundManager.instance.PlaySound(SoundManager.Sound.HintNotif);
            gameManager.HintMessage(hintMsg,msgTime);
        }
    }
}
