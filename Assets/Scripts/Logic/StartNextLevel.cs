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
    private void Awake()
    {
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
        multiSceneVariables = GameObject.FindGameObjectWithTag("MultiSceneVariables").GetComponent<MultiSceneVariables>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3) StartCoroutine(StartSpecifiedLevel());
    }
    public IEnumerator StartSpecifiedLevel()
    {
        if (multiSceneVariables != null) multiSceneVariables.FinishLevel(currentLevel);
        logic.StartScreenFade();
        yield return new WaitForSeconds(timeToWait + logic.ReturnDelayForEndScreen());
        if (multiSceneVariables != null) multiSceneVariables.fullyRestartLevel();
        logic.StartLevel(levelToLoad);
    }
}
