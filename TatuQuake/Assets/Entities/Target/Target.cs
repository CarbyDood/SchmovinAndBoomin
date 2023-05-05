using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private float health = 50f;
    bool isShaking = false;

    float shakingTime = 1f;
    float timer = 0f;

    [SerializeField] GameObject obj;
    [SerializeField] GameObject parentObj;

    //keep track of target's original position
    Vector3 ogPos;

    public void TakeDamage(float amount)
    {
        health -= amount;
        isShaking = true;
        timer = 0f;

        Debug.Log("Damage Taken: "+amount);
        if (health <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void Start()
    {
        ogPos = parentObj.transform.position;
    }

    private void Update()
    {
        if (isShaking)
        {
            timer += Time.deltaTime;
            if(timer < shakingTime)
            {
                float x = parentObj.transform.position.x;
                float y = parentObj.transform.position.y;
                float z = ogPos.z + (Mathf.Sin(Time.time * 750f) * 0.10f);
                parentObj.transform.position = new Vector3(x, y, z);
            }
            else {
                isShaking = false;
            }
        }

        else
        {
            parentObj.transform.position = ogPos;
        }
    }
}
