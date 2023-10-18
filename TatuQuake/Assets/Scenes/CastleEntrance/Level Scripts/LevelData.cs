using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    public int secrets {get; set;}
    public int secretsFound {get; set;}
    public int enemies {get; set;}
    public int enemiesKilled {get; set;}
    public float time {get; set;}
    public string levelName;

    [SerializeField] private Transform secretsParent;
    [SerializeField] private Transform enemiesParent;

    // Start is called before the first frame update
    void Start()
    {
        secrets = secretsParent.childCount;
        enemies = enemiesParent.childCount;
    }
}
