using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class TargetTrigger : MonoBehaviour
{
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

    public void OnHit() 
    {
        if(moveDoor == false)
        {
            moveDoor = true;
            transform.position += -transform.up*1f;
        }
    }
}
