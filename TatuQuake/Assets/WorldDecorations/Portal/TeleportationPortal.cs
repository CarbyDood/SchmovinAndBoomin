using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TeleportationPortal : MonoBehaviour
{
    [SerializeField] private Vector3 playerDest;
    private PlayerMovement player;

    private void Start() 
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Player"))
        {
            SoundManager.instance.PlaySound(SoundManager.Sound.Teleport);
            player.MovePosition(playerDest);
        }
    }
}
