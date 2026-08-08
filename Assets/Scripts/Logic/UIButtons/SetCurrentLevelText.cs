using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetCurrentLevelText : MonoBehaviour
{
    private void Awake()
    {
        TextMeshProUGUI myText = GetComponent<TextMeshProUGUI>();
        myText.text = SceneManager.GetActiveScene().name;
    }
}
