using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SMGPickUpTrigger : MonoBehaviour
{
    private GameManager gameManager;
    [SerializeField] private GameObject doorToAffect;
    [SerializeField] private GameObject lightToTurnoff;
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
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Player")&& moveDoor == false)
        {
            lightToTurnoff.SetActive(false);
            StartCoroutine(MoveDoor());
        }
    }

    public IEnumerator MoveDoor()
    {
        yield return new WaitForSeconds(2);
        moveDoor = true;
    }
}
