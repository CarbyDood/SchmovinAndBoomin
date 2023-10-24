using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePad : MonoBehaviour
{
    private GameManager gameManager;
    private PlayerMovement player;
    [SerializeField] private float forceToApply;

    void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.TryGetComponent(out player))
        {
            SoundManager.instance.PlaySound(SoundManager.Sound.Boing);
            player.ApplyForce(forceToApply, Vector3.up, 1);
        }
    }
}
