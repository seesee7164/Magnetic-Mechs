using UnityEngine;

public class MechBossTriggerScript : MonoBehaviour
{
    //triggers the robot spider queen boss fight by closing walls, activating the boss, and updating the camera
    [Header("Components")]
    public MechBossControllerScript mechBossControllerScript;
    //public GoomechSpawnerScript goomechSpawnerScriptTop;
    //public GoomechSpawnerScript goomechSpawnerScriptBottom;
    public GameObject bossCanvas;
    //public GameObject entrances;
    public GameObject virtualCameraPlayer;
    public GameObject virtualCameraBoss;
    //public AudioSource audioSource;
    //public AudioManager audioManager;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (bossCanvas == null || mechBossControllerScript == null || virtualCameraPlayer == null || virtualCameraBoss == null) return;
        if (collision.gameObject.layer == 3)
        {
            mechBossControllerScript.ActivateBoss();
            //goomechSpawnerScriptTop.bossActive = true;
            //goomechSpawnerScriptBottom.bossActive = true;
            bossCanvas.SetActive(true);
            //entrances.SetActive(true);
            //virtualCamera.Follow = cameraPosition.transform;
            //virtualCamera.m_Lens.OrthographicSize = 10;
            virtualCameraPlayer.SetActive(false);
            virtualCameraBoss.SetActive(true);
            //AudioClip loadedClip = Resources.Load<AudioClip>("BackgroundMusic/the_robot_spider_queen_invasion_Part2");
            //if (loadedClip != null)
            //{
            //    StartCoroutine(SwapMusic(loadedClip));
            //}
        }
    }
    //public IEnumerator SwapMusic(AudioClip loadedClip)
    //{
    //    //audioManager.fade(3f);
    //    //Debug.Log("test1");
    //    yield return new WaitForSeconds(.01f);
    //    //Debug.Log("test2");
    //    //audioManager.stopFade();
    //    audioSource.clip = loadedClip;
    //    audioSource.Play();
    //    turnOffGameObject();
    //}
    private void turnOffGameObject()
    {
        gameObject.SetActive(false);
    }
}

