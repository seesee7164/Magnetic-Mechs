using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartNextLevel : MonoBehaviour
{
    public string levelToLoad = "fill in here";
    private float timeToWait = 1.75f;
    public int currentLevel = 1;
    [Header("Components")]
    private MultiSceneVariables multiSceneVariables;
    private LogicScript logic;
    private PlayerHealthScript playerHealthScript;

    private void Awake()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
        multiSceneVariables = GameObject.FindGameObjectWithTag("MultiSceneVariables").GetComponent<MultiSceneVariables>();
        playerHealthScript = GameObject.FindGameObjectWithTag("PlayerHealth").GetComponent<PlayerHealthScript>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3) StartCoroutine(StartSpecifiedLevel());
    }
    public IEnumerator StartSpecifiedLevel()
    {
        if (multiSceneVariables != null) multiSceneVariables.FinishLevel(currentLevel);
        if(playerHealthScript != null) playerHealthScript.invincible = true;
        logic.StartScreenFade();
        yield return new WaitForSeconds(timeToWait + logic.ReturnDelayForEndScreen());
        if (multiSceneVariables != null) multiSceneVariables.fullyRestartLevel();
        logic.StartLevel(levelToLoad);
    }
}
