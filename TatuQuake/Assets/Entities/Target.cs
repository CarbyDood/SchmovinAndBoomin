using UnityEngine;

public class Target : MonoBehaviour
{
    private float health = 50f;
    bool isShaking = false;

    float shakingTime = 1f;
    float timer = 0f;

    [SerializeField] GameObject obj;

    //keep track of target's original position
    Vector3 ogPos;

    public void TakeDamage(float amount)
    {
        health -= amount;
        isShaking = true;
        timer = 0f;

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
        ogPos = obj.transform.position;
    }

    private void Update()
    {
        if (isShaking)
        {
            timer += Time.deltaTime;
            if(timer < shakingTime)
            {
                float x = obj.transform.position.x;
                float y = obj.transform.position.y;
                float z = ogPos.z + (Mathf.Sin(Time.time * 750f) * 0.10f);
                obj.transform.position = new Vector3(x, y, z);
            }
            else {
                isShaking = false;
            }
        }

        else
        {
            obj.transform.position = ogPos;
        }
    }
}
