using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayLevelTimeScript : MonoBehaviour
{
    [Header("Variables")]
    public int level;
    [Header("Components")]
    private MultiSceneVariables multiSceneVariables;
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
        displayTime.text = multiSceneVariables.GetSavedLevelTime(level);
    }
}
