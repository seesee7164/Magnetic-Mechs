using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Internal.Filters;
using UnityEngine;
public class FreeMovingBeeDroneSpawnerScript : MonoBehaviour
{
    [Header("Number of Drones")]
    private GameObject[] beeDronesArray;
    private Queue<int> beeDronesAvailableQueue;
    private int maxBeeDrones = 1;
    [Header("Manage Spawn")]
    private float timeToSpawn;
    private float timer;
    public bool active = false;
    [Header("Components")]
    public GameObject beeDronePrefab;
    public Transform playerTransform;
    public Transform myTransform;
    [Header("Spawn Points")]
    public Vector3 spawnPosition;
    public bool spawnRight;
    private float spawnRightChance = .5f;
    private float spawnRightChanceModifier = .1f;
    [Header("Variables")]
    public bool uniformSpeed = false;
    public bool followX = false;
    public bool followY = false;
    private float heightSpread = 2f;
    public float XSpawnDistance = 12f;
    //public float MaxXDistance = 15f;
    //public float MaxYDistance = 15f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timeToSpawn = 2;
        myTransform = transform;
        SetUpArrays();
    }
    void FixedUpdate()
    {
        if (playerTransform != null)
        {
            float newXPos = followX ? playerTransform.position.x : myTransform.position.x;
            float newYPos = followY ? playerTransform.position.y : myTransform.position.y;
            myTransform.position = new Vector3(newXPos, newYPos, myTransform.position.z);
        }
        if (!active) return;
        if (beeDronesAvailableQueue.Count == 0)
        {
            timer = 0;
        }
        if (timer < timeToSpawn)
        {
            timer = timer + Time.fixedDeltaTime;
        }
        else
        {
            Vector3 height = new Vector3(0f, Random.Range(-1.0f, 1.0f) * heightSpread, 0f);
            Vector3 xDistance = new Vector3(XSpawnDistance, 0, 0);
            spawnPosition = transform.position + xDistance + height; //remove this and the following line when making it spawn from either direction
            spawnRight = true;
            //spawnRight = UnityEngine.Random.Range(0f, 1f) < spawnRightChance;
            //if (spawnRight)
            //{
            //    spawnPosition = transform.position + xDistance + height;
            //    if (spawnRightChance < .5f)
            //    {
            //        spawnRightChance = .5f - spawnRightChanceModifier;
            //    }
            //    else
            //    {
            //        spawnRightChance -= spawnRightChanceModifier;
            //    }
            //}
            //else
            //{
            //    spawnPosition = transform.position - xDistance + height;
            //    if (spawnRightChance > .5f)
            //    {
            //        spawnRightChance = .5f + spawnRightChanceModifier;
            //    }
            //    else
            //    {
            //        spawnRightChance += spawnRightChanceModifier;
            //    }
            //}
            spawnDrone();
            timer = 0;
        }
    }
    private void SetUpArrays()
    {
        beeDronesArray = new GameObject[maxBeeDrones];
        for (int i = 0; i < maxBeeDrones; i++)
        {
            GameObject tempBeeDrone = Instantiate(beeDronePrefab, transform.position, Quaternion.Euler(0, 180, 0));
            beeDronesArray[i] = tempBeeDrone;
            tempBeeDrone.SetActive(false);
            FreeMovingBeeDroneScript tempDroneScript = tempBeeDrone.GetComponent<FreeMovingBeeDroneScript>();
            tempDroneScript.beeDroneRespawnerScript = this;
        }
        beeDronesAvailableQueue = new Queue<int>();
        for (int i = 0; i < maxBeeDrones; i++)
        {
            beeDronesAvailableQueue.Enqueue(i);
        }
    }
    public void Activate()
    {
        active = true;
        for(int i = 0; i < maxBeeDrones; i++)
        {
            beeDronesArray[i].GetComponent<FreeMovingBeeDroneScript>().despawnBehavior = false;
        }
    }
    public void Deactivate()
    {
        active = false;
        for (int i = 0; i < maxBeeDrones; i++)
        {
            beeDronesArray[i].GetComponent<FreeMovingBeeDroneScript>().despawnBehavior = true;
        }
    }
    void spawnDrone()
    {
        int currentIndex = beeDronesAvailableQueue.Dequeue();
        GameObject tempBeeDrone = beeDronesArray[currentIndex];
        tempBeeDrone.SetActive(true);
        tempBeeDrone.transform.position = spawnPosition;
        FreeMovingBeeDroneScript tempBeeDroneScript = tempBeeDrone.GetComponent<FreeMovingBeeDroneScript>();
        tempBeeDroneScript.index = currentIndex;
        tempBeeDroneScript.restartDrone();
        if (spawnRight ^ !tempBeeDroneScript.facingRight)
        {
            tempBeeDroneScript.Flip();
        }
    }
    public void DroneKilled(int index)
    {
        beeDronesAvailableQueue.Enqueue(index);
    }
}
