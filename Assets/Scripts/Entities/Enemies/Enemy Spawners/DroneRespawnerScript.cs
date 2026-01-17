using UnityEngine;
using System.Collections.Generic;


public class DroneRespawnerScript : MonoBehaviour
{
    //script responsible for spawning drones
    [Header("Number of Drones")]
    private GameObject[] DronesArray;
    private Queue<int> DronesAvailableQueue;
    private int maxDrones = 2;
    [Header("Manage Spawn")]
    private float timeToSpawn;
    private float timer;
    [Header("Components")]
    public GameObject dronePrefab;
    public Transform playerTransform;
    public Transform myTransform;
    [Header("Spawn Points")]
    public Vector3 spawnPosition;
    [Header("Variables")]
    public bool uniformSpeed = false;
    public Transform minYPosition;
    public Transform maxYPosition;
    private bool disabled = false;
    private void Start()
    {
        timeToSpawn = 6.5f;
        timer = timeToSpawn - 3.0f;
        myTransform = transform;
        SetUpArrays();
    }
    private void SetUpArrays()
    {
        DronesArray = new GameObject[maxDrones];
        DronesAvailableQueue = new Queue<int>();
        for (int i = 0; i < maxDrones; i++)
        {
            GameObject tempDrone = Instantiate(dronePrefab, transform.position, Quaternion.Euler(0, 0, 0));
            tempDrone.transform.parent = this.transform;
            tempDrone.transform.rotation = Quaternion.Euler(0, 180, 0);
            DronesArray[i] = tempDrone;
            if (uniformSpeed) tempDrone.GetComponent<DroneHorizontalScript>().speedModifier = 0f;
            tempDrone.SetActive(false);
            FlyingEnemy tempDroneScript = tempDrone.GetComponent<FlyingEnemy>();
            tempDroneScript.droneRespawnerScript = this;
            // tempDroneScript.DroneSpawnerTransform = transform;
            tempDroneScript.lowerVerticalBound = minYPosition;
            tempDroneScript.upperVerticalBound = maxYPosition;
            tempDroneScript.switchTime = 4f;
        }
        DronesAvailableQueue.Enqueue(0);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if(disabled) return;
        if (DronesAvailableQueue.Count == 0)
        {
            timer = 0;
        }
        else if (timer < timeToSpawn)
        {
            timer = timer + Time.fixedDeltaTime;
        }
        else
        {
            spawnDrone();
            timer = 0;
        }
    }
    void spawnDrone()
    {
        int currentIndex = DronesAvailableQueue.Dequeue();
        spawnPosition = transform.position + new Vector3(-currentIndex, 0, 0);
        GameObject tempDrone = DronesArray[currentIndex];
        tempDrone.SetActive(true);
        tempDrone.transform.position = spawnPosition;
        FlyingEnemy tempDroneScript = tempDrone.GetComponent<FlyingEnemy>();
        tempDroneScript.index = currentIndex;
        tempDroneScript.restartDrone();
        tempDroneScript.enabled = true;
    }

    public void DroneKilled(int index)
    {
        DronesAvailableQueue.Enqueue(index);
        DronesArray[index].gameObject.SetActive(false);
    }
    public void ActivatePhase2()
    {
        DronesAvailableQueue.Enqueue(1);
        timeToSpawn = 5.5f;
    }
    public void BossDied()
    {
        disabled = true;
    }
}
