using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlankScript : MonoBehaviour
{
    //script for managing dropping the player through wooden platforms
    [Header("Layers")]
    private LayerMask parentMechLayer;
    private LayerMask plankGroundLayer;
    [Header("Variables")]
    public float disableCollisionTimer;
    public float disablePlankTime = .25f;
    public bool isPlayerMech = true;
    void Awake()
    {
        if(isPlayerMech) parentMechLayer = LayerMask.NameToLayer("Player");
        else parentMechLayer = LayerMask.NameToLayer("Enemy Mech Boss");
        plankGroundLayer = LayerMask.NameToLayer("Plank Ground");
        disableCollisionTimer = disablePlankTime;
    }
    void FixedUpdate()
    {
        if(disableCollisionTimer < disablePlankTime)
        {
            Physics2D.IgnoreLayerCollision(parentMechLayer, plankGroundLayer, true);
            disableCollisionTimer += Time.fixedDeltaTime;
        }
        else
        {
            Physics2D.IgnoreLayerCollision(parentMechLayer, plankGroundLayer, false);
        }
    }
    public void disablePlanks()
    {
        disableCollisionTimer = 0;
    }
}
