using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateDroneSpawnerScript : MonoBehaviour
{
    //turns on the drone spawner when the player collides with it
    public DroneSpawnerScript droneSpawnerScript;
    // Start is called before the first frame update
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3 && droneSpawnerScript != null) droneSpawnerScript.Activate();
    }
}