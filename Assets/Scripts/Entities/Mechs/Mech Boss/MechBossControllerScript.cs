using UnityEngine;

public class MechBossControllerScript : MonoBehaviour
{
    [Header("Components")]
    public MechBossActionsScript availableActionsScript;
    [Header("variables")]
    private bool bossActive = false;
    private bool trackAndShootPlayer = true;

    private void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {
        if (!bossActive) return;
        if (trackAndShootPlayer) availableActionsScript.trackPlayerWithGun();
    }
    public void ActivateBoss()
    {
        bossActive = true;
        startTrackingAndShootingPlayer();
        availableActionsScript.bulletSpawnerScript.ResetTimer();
    }
    private void startTrackingAndShootingPlayer()
    {
        trackAndShootPlayer = true;
        availableActionsScript.StartShooting();
    }
}
