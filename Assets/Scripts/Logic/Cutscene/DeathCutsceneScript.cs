using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DeathCutsceneScript : MonoBehaviour
{
    [Header("Components")]
    public Sprite[] cutsceneImages;
    public GameObject imageGameObject;
    public Image imageDisplay;
    public TutorialAllEvents tutorialAllEvents;
    [Header("Variables")]
    private float FPS = 4;

    public void startPlaying()
    {
        StartCoroutine(startPlayingCoroutine());
    }
    public IEnumerator startPlayingCoroutine()
    {
        //Debug.Log("it Worked");
        imageGameObject.SetActive(true);
        for (int i = 0; i < cutsceneImages.Length; i++)
        {
            imageDisplay.sprite = cutsceneImages[i];
            yield return new WaitForSeconds(1 / FPS);
            Debug.Log(i);
        }
        imageGameObject.SetActive(false);
        tutorialAllEvents.afterCutscene();
    }
}
