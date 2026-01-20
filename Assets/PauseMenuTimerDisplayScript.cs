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
        Debug.Log("updateScript");
        myTime.text = multiSceneVariables.returnCurrentTimeAsString();
    }
}
