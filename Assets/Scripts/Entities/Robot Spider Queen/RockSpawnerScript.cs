using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSpawnerScript : MonoBehaviour
{
    //script for spawning rocks during the robot spider queens second phase
    [Header("Components")]
    public GameObject rockSpawnerPrefab;
    [Header("Manage Spawn")]
    public float timeToSpawn = 3;
    //public float despawnHeight = 0;
    private float timer;
    [Header("Stages")]
    private bool dontSpawn = true;
    // Start is called before the first frame update
    [Header("Manage Queue")]
    private GameObject[] RocksArray;
    private Queue<int> RocksAvailableQueue;
    private int maxRocks = 2;
    [Header("variables")]
    public int spawnwidth = 48;
    public float minHeight = -3f;
    public float speed = 3f;
    void Start()
    {
        timer = 0;
        SetUpArrays();
    }
    private void SetUpArrays()
    {
        RocksArray = new GameObject[maxRocks];
        for (int i = 0; i < maxRocks; i++)
        {
            GameObject tempRockSpawner = Instantiate(rockSpawnerPrefab, transform.position, Quaternion.Euler(0, 0, 0));
            RocksArray[i] = tempRockSpawner;
            tempRockSpawner.transform.parent = transform;
            tempRockSpawner.SetActive(false);
            GameObject tempRock = tempRockSpawner.GetComponent<IndividualRockSpawnerScript>().returnRock();
            RockScript rockScript = tempRock.GetComponent<RockScript>();
            rockScript.RockSpawnerScript = this;
            rockScript.index = i;
            rockScript.minHeight = minHeight;
            rockScript.transform.parent = transform;
        }
        RocksAvailableQueue = new Queue<int>();
        for (int i = 0; i < maxRocks; i++)
        {
            RocksAvailableQueue.Enqueue(i);
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (dontSpawn)
        {
            return;
        }
        if (timer < timeToSpawn || RocksAvailableQueue.Count == 0)
        {
            timer = timer + Time.fixedDeltaTime;
        }
        else
        {
            float spawnX = Random.value * spawnwidth;
            SpawnRock(transform.position.x + spawnX - spawnwidth/2);
            timer = 0;
        }
    }
    void SpawnRock(float spawnX)
    {
        int currentIndex = RocksAvailableQueue.Dequeue();
        GameObject tempRockSpawner = RocksArray[currentIndex];
        tempRockSpawner.SetActive(true);
        tempRockSpawner.transform.position = new Vector3(spawnX, transform.position.y, .5f);
        tempRockSpawner.GetComponent<IndividualRockSpawnerScript>().startSpawningRock();
    }
    public void RockDestroyed(int index)
    {
        RocksArray[index].SetActive(false);
        RocksAvailableQueue.Enqueue(index);
    }
    public void TriggerStage2()
    {
        dontSpawn = false;
    }
    public void BossDied()
    {
        dontSpawn = true;
    }
}
