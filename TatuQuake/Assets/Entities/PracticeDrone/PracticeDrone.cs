using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PracticeDrone : MonoBehaviour
{
    private float zRot = 0f;
    private float maxRot = 15f;
    [SerializeField] float rotSpeed = 1f;
    [SerializeField] float attackSpeed = 1f;
    [SerializeField] int damage = 10;
    [SerializeField] float impactForce = 50f;
    private float range = 100f;

    [SerializeField] protected TrailRenderer entityTrail;
    [SerializeField] protected GameObject impactEffect;

    private float timer = 0f;
    private float interval;

    // Start is called before the first frame update
    void Start()
    {
        interval = 1f/attackSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        zRot = Mathf.Sin(Time.time * rotSpeed) * maxRot;
        transform.rotation = Quaternion.Euler(0, -180, zRot);

        if(timer >= interval)
        {
            timer -= interval;
            SoundManager.instance.PlaySound(SoundManager.Sound.PistolShot, transform.position);
            Attack();
        }

        timer += Time.deltaTime;
    }

    void Attack()
    {
        RaycastHit hit;

        if(Physics.Raycast(transform.position, transform.forward, out hit, range))
        {
            //spawn bullet trail
            TrailRenderer entityViewTrail = Instantiate(entityTrail, transform.position, Quaternion.identity);
            StartCoroutine(SpawnTrail(entityViewTrail, hit.point));

            Target target = hit.transform.GetComponent<Target>();
            if(target != null)
            {
                target.TakeDamage(damage);
            }

            PlayerMovement player = hit.transform.GetComponent<PlayerMovement>();
            if(player != null)
            {
                player.TakeDamage(damage);
            }

            if (hit.rigidbody != null){
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }

            Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
        }

        //if we hit nothing, show a trail towards the camera's center at a distance of the weapon's range
        else
        {
            TrailRenderer entityViewTrail = Instantiate(entityTrail, transform.position, Quaternion.identity);
            Vector3 pointTo = transform.position + (transform.forward * range);
            StartCoroutine(SpawnTrail(entityViewTrail, pointTo));
        }
    }

    public IEnumerator SpawnTrail(TrailRenderer trail, Vector3 dest)
    {
        float time = 0;
        Vector3 startPos = trail.transform.position;

        while (time < 0.5)
        {
            trail.transform.position = Vector3.Lerp(startPos, dest, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }
        trail.transform.position = dest;

        Destroy(trail.gameObject, trail.time);
    }
}
