using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorPickUp : MonoBehaviour
{
    private GameManager gameManager;

    private float bobHeight = 0.1f;
    private float bobSpeed = 3f;
    private float ogPosY;
    private float yRot = 0f;
    
    private bool isTouchingPlayer = false;
    private PlayerMovement player;

    [SerializeField] private bool canRespawn;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
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
        transform.rotation = Quaternion.Euler(-90, yRot, 0);

        if(isTouchingPlayer)
        {
            if(player.GetArmour() < player.GetMaxArmour())
            {
                player.GiveArmour(50);

                SoundManager.instance.PlaySound(SoundManager.Sound.ArmorPickUp);
                gameManager.ConsoleMessage("Get equipped with 50 armour");

                if(canRespawn) gameManager.DisableObjectForTime(gameObject, 5);
                else Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.TryGetComponent(out player))
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
