using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampWellDoor : MonoBehaviour
{
    [SerializeField] private Vector3 doorDest;
    [SerializeField] private float SpeedOfOpening = 1;
    [SerializeField] private GameObject hint2Box;
    private int doorCounter = 0;

    // Update is called once per frame
    void Update()
    {
        if(doorCounter >= 4 && transform.position != doorDest)
        {
            transform.position = Vector3.MoveTowards(transform.position, doorDest, SpeedOfOpening * Time.deltaTime);
            hint2Box.SetActive(false);
        }
    }

    public void incrementCounter()
    {
        doorCounter++;
    }

    public int getCounter()
    {
        return doorCounter;
    }
}
