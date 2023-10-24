using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampWellSwitch : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField] private GameObject switchLight;
    [SerializeField] private RampWellDoor doorToAffect;
    private bool hit = false;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "Player" && hit == false)
        {
            doorToAffect.incrementCounter();
            switchLight.SetActive(true);
            SoundManager.instance.PlaySound(SoundManager.Sound.HintNotif);
            if(doorToAffect.getCounter() == 4)
                gameManager.HintMessage("Sequence Complete!",3);
            else
                gameManager.HintMessage($"There are {4 - doorToAffect.getCounter()} switches left...",3);
            hit = true;
        }
    }
}
