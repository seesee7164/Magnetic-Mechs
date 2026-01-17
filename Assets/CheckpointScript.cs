using Cinemachine;
using UnityEngine;

public class CheckpointScript : MonoBehaviour
{
    [Header("Components")]
    private MultiSceneVariables multiSceneVariables;
    private GameObject player;
    public CinemachineVirtualCamera virtualCamera;
    private PlayerHealthScript playerHealthScript;
    private AudioSource myAudioSource;
    [Header("variable")]
    public int checkpoint;
    private bool hasTriggered;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        multiSceneVariables = GameObject.FindGameObjectWithTag("MultiSceneVariables").GetComponent<MultiSceneVariables>();
        playerHealthScript = GameObject.FindGameObjectWithTag("PlayerHealth").GetComponent<PlayerHealthScript>();
        player = GameObject.FindGameObjectWithTag("Player");
        myAudioSource = GetComponent<AudioSource>();
        hasTriggered = false;
        if (multiSceneVariables != null && player != null && checkpoint != 0)
        {
            if(multiSceneVariables.getCheckpoint() == checkpoint)
            {
                hasTriggered = true;
                float myX = transform.position.x;
                float myY = transform.position.y;
                player.transform.position = new Vector3(myX,myY, player.transform.position.z);
                if (virtualCamera != null) virtualCamera.transform.position = player.transform.position;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(multiSceneVariables.getCheckpoint());
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            if (multiSceneVariables != null && checkpoint != 0 && !hasTriggered)
            {
                hasTriggered = true;
                multiSceneVariables.setCheckpoint(checkpoint);
                myAudioSource.Play();
                playerHealthScript.healToFull();
            }
        }

    }
}
