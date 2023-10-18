using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretDoorButton : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private GameObject doorToAffect;
    [SerializeField] private Vector3 doorDest;
    bool moveDoor = false;
    [SerializeField] private float SpeedOfOpening = 1;

    private void Update() 
    {
        if(moveDoor == true && doorToAffect.transform.position != doorDest)
        {
            doorToAffect.transform.position = Vector3.MoveTowards(doorToAffect.transform.position, doorDest, SpeedOfOpening * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.tag == "Player" && moveDoor == false)
        {
            transform.position += -transform.up*0.075f;
            moveDoor = true;
        }
    }
}
