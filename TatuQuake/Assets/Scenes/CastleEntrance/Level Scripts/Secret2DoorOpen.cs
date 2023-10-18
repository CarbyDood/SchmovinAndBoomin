using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Secret2DoorOpen : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField] private GameObject doorToAffect;
    [SerializeField] private Vector3 doorDest;
    private Vector3 doorOGPos;
    bool moveDoor = false;
    [SerializeField] private float SpeedOfOpening = 1;

    private void Start() 
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        doorOGPos = doorToAffect.transform.position;
    }

    private void Update() 
    {
        if(moveDoor == true && doorToAffect.transform.position != doorDest)
        {
            doorToAffect.transform.position = Vector3.MoveTowards(doorToAffect.transform.position, doorDest, SpeedOfOpening * Time.deltaTime);
        }

        else if(moveDoor == false && doorToAffect.transform.position != doorOGPos)
        {
            doorToAffect.transform.position = Vector3.MoveTowards(doorToAffect.transform.position, doorOGPos, SpeedOfOpening * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Player")&& moveDoor == false)
        {
            moveDoor = true;
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        if(other.CompareTag("Player"))
        {
            StartCoroutine(MoveDoorBack());
        }
    }

    public IEnumerator MoveDoorBack()
    {
        yield return new WaitForSeconds(3);
        moveDoor = false;
    }
}
