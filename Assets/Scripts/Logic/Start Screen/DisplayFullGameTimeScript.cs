using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class DisplayFullGameTimeScript : MonoBehaviour
{
    //[Header("Variables")]
    [Header("Components")]
    private MultiSceneVariables multiSceneVariables;
    //private Text displayTime;
    private TextMeshProUGUI displayTime;
    private void Awake()
    {
        displayTime = GetComponent<TextMeshProUGUI>();
        multiSceneVariables = GameObject.FindGameObjectWithTag("MultiSceneVariables").GetComponent<MultiSceneVariables>();
        SetDisplayTime();
    }

    public void SetDisplayTime()
    {
        if (multiSceneVariables == null) return;
        string clearTime = multiSceneVariables.GetSavedLevelTime(0);
        if (clearTime == "")
        {
            displayTime.text = "";
            return;
        }
        displayTime.text = "Fastest Clear: " + clearTime;
    }
}
