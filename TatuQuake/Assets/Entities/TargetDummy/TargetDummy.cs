using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetDummy : MonoBehaviour
{
    public List<Collider> ragdollParts = new List<Collider>();

    // Start is called before the first frame update
    void Start()
    {
        SetRagdollParts();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void SetRagdollParts()
    {
        //Also disables ragdoll stuff
        Collider[] collider = this.gameObject.GetComponentsInChildren<Collider>();

        foreach(Collider c in collider)
        {
            if(c.gameObject != this.gameObject)
            {
                c.isTrigger = true;
                c.attachedRigidbody.isKinematic = true;
                ragdollParts.Add(c);
            }
        }
    }

    public void TurnOnRagdoll()
    {
        gameObject.GetComponent<BoxCollider>().enabled = false;
        foreach(Collider c in ragdollParts)
        {
            c.isTrigger = false;
            c.attachedRigidbody.isKinematic = false;
            c.attachedRigidbody.velocity = Vector3.zero;
        }
    }
}
