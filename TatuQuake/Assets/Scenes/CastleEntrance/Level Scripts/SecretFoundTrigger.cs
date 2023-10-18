using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretFoundTrigger : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    private bool SecretFound = false;

    private void Start() 
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Player") && !SecretFound)
        {
            SoundManager.instance.PlaySound(SoundManager.Sound.SecretFound);
            gameManager.HintMessage("Secret Found!",5);
            SecretFound = true;
            gameManager.SecretFound();
        }
    }
}
