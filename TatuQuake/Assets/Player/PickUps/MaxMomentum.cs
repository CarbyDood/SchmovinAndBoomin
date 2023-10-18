using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxMomentum : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    private float bobHeight = 0.1f;
    private float bobSpeed = 3f;
    private float ogPosY;
    private float yRot = 0f;

    private float duration = 10f;

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
        transform.rotation = Quaternion.Euler(0, yRot, 0);
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "Player")
        {
            PlayerMovement player = other.transform.GetComponent<PlayerMovement>();

            SoundManager.instance.PlaySound(SoundManager.Sound.MaxMomentumPickUp);
            gameManager.ConsoleMessage("Imbue with the dew... Max Momentum activated!");
            gameManager.UpdateStatus(GameManager.Status.MaxMomentumActive);
            player.PowerUp(PlayerMovement.PowerUps.MaxMomentum, duration);

            if(canRespawn) gameManager.DisableObjectForTime(gameObject, 1);
            else Destroy(gameObject);
        }
    }
}
