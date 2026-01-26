using UnityEngine;
using UnityEngine.UI;

public class PauseMenuTimerDisplayScript : MonoBehaviour
{
    [Header("Components")]
    private MultiSceneVariables multiSceneVariables;
    private Text myTime;
    void Awake()
    {
        multiSceneVariables = GameObject.FindGameObjectWithTag("MultiSceneVariables").GetComponent<MultiSceneVariables>();
        myTime = GetComponent<Text>();
    }

    void Update()
    {
        //Debug.Log("updateScript");
        string newText = "Current Level" + System.Environment.NewLine + multiSceneVariables.returnCurrentTimeAsString();
        if (multiSceneVariables.StartedWithLevelOne())
        {
            newText = newText + System.Environment.NewLine + "Full Game Time" + System.Environment.NewLine + multiSceneVariables.returnFullGameTimeAsString();
        }
        myTime.text = newText;
    }
}
