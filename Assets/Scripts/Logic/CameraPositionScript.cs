using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPositionScript : MonoBehaviour
{
    //sets the position of the an object to a spot between the player and the boss so that the camera can follow it
    [Header("Components")]
    public Transform playerTransform;
    public Transform bossTransform;

    private void FixedUpdate()
    {
        if (playerTransform == null || bossTransform == null) return;
        transform.position = (playerTransform.position*1.8f + bossTransform.position*1.2f)/3;
    }
}
