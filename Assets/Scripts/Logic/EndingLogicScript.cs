using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class EndingLogicScript : MonoBehaviour
{
    [Header("Components")]
    private MultiSceneVariables multiSceneVariables;
    public Text FullGameFinishTime;
    void Awake()
    {
        multiSceneVariables = GameObject.FindGameObjectWithTag("MultiSceneVariables").GetComponent<MultiSceneVariables>();
    }

    private void Update()
    {
        if (!multiSceneVariables.StartedWithLevelOne()) return;
        int prevTime = multiSceneVariables.returnPreviousGameTime();
        if (prevTime == 0)
        {
            FullGameFinishTime.text = "Full Time: " + multiSceneVariables.returnFullGameTimeAsString();
        }
        else
        {
            FullGameFinishTime.text = "Full Time: " + multiSceneVariables.returnFullGameTimeAsString() + Environment.NewLine + "Previous Best: " + multiSceneVariables.returnPrevGameTimeAsString();
        }
    }
    public void ReturnToMainMenu()
    {
        multiSceneVariables.fullyRestartLevel();
        SceneManager.LoadScene("Main Menu");
    }
}
