using UnityEngine;

public class FreeMovingBeeDroneActivation : MonoBehaviour
{
    public bool turnOn = true;
    //turns on the drone spawner when the player collides with it
    public FreeMovingBeeDroneSpawnerScript beeDroneSpawnerScript;
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3 && beeDroneSpawnerScript != null)
        {
            if (turnOn) beeDroneSpawnerScript.Activate();
            else beeDroneSpawnerScript.Deactivate();
        }
    }
}
