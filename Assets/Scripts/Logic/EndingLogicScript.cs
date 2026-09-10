using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using TMPro;

public class EndingLogicScript : MonoBehaviour
{
    [Header("Components")]
    private MultiSceneVariables multiSceneVariables;
    public TextMeshProUGUI FullGameFinishTime;
    public GameObject NotFullPlayThroughObjects;
    public GameObject FullPlayThroughObjects;
    void Awake()
    {
        multiSceneVariables = GameObject.FindGameObjectWithTag("MultiSceneVariables").GetComponent<MultiSceneVariables>();
    }

    private void Update()
    {
        if (!multiSceneVariables.StartedWithLevelOne())
        {
            NotFullPlaythrough();
            return;
        }
        FullPlaythrough();
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
    private void NotFullPlaythrough()
    {
        NotFullPlayThroughObjects.SetActive(true);
        FullPlayThroughObjects.SetActive(false);
    }
    private void FullPlaythrough()
    {
        NotFullPlayThroughObjects.SetActive(false);
        FullPlayThroughObjects.SetActive(true);
    }
    public void ReturnToMainMenu()
    {
        multiSceneVariables.fullyRestartLevel();
        SceneManager.LoadScene("Main Menu");
    }
}
