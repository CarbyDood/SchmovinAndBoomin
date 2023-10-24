using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    private GameManager gameManager;
    private PlayerMovement player;
    [SerializeField] private GameObject continueText;
    private bool canContinue = false;
    
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
    }

    private void Update() 
    {
        if(player.select.triggered && canContinue)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("Player"))
        {
            player.LevelWon();
            StartCoroutine(PlayerContinue());
        }
    }

    public IEnumerator PlayerContinue()
    {
        yield return new WaitForSeconds(3);
        continueText.SetActive(true);
        canContinue = true;
    }
}
