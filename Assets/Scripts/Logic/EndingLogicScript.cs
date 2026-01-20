using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        FullGameFinishTime.text = "Full Time: " + multiSceneVariables.returnFullGameTimeAsString();
    }
    public void ReturnToMainMenu()
    {
        multiSceneVariables.fullyRestartLevel();
        SceneManager.LoadScene("Main Menu");
    }
}
