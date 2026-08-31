using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthHeartScript : MonoBehaviour
{
    //script for managing heart containers on the UI
    private GameObject[] heartContainers;
    private Image[] heartFills;
    [Header("Components")]
    public Transform heartsParent;
    public GameObject heartContainerPrefab;
    public GameObject PlayerHealth;

    private void Start()
    {
        PlayerHealth = GameObject.FindGameObjectWithTag("PlayerHealth");
        if (PlayerHealth == null )
        {
            Debug.Log("PlayerHealth Could not be found");
            return;
        }
        int maxPossibleHealth = (int)PlayerHealth.GetComponent<PlayerHealthScript>().maxPossibleHealth;
        heartContainers = new GameObject[maxPossibleHealth];
        heartFills = new Image[maxPossibleHealth];

        InstantiateHeartContainers();
        UpdateHeartsHUD();
    }
    void InstantiateHeartContainers()
    {
        for (int i = 0; i < PlayerHealth.GetComponent<PlayerHealthScript>().maxPossibleHealth; i++)
        {
            GameObject temp = Instantiate(heartContainerPrefab);
            temp.transform.SetParent(heartsParent, false);
            heartContainers[i] = temp;
            heartFills[i] = temp.transform.Find("HeartFill").GetComponent<Image>();
        }
    }
    public void UpdateHeartsHUD()
    {
        if (PlayerHealth == null)
        {
            Debug.Log("PlayerHealth Could not be found");
            return;
        }
        SetHeartContainers();
        SetFilledHearts();
    }
    void SetHeartContainers()
    {
        int maxHealth = (int)PlayerHealth.GetComponent<PlayerHealthScript>().getMaxHealth();
        for (int i = 0; i < heartContainers.Length; i++)
        {
            if (i < maxHealth)
            {
                heartContainers[i].SetActive(true);
            }
            else
            {
                heartContainers[i].SetActive(false);
            }
        }
    }
    void SetFilledHearts()
    {
        float currHealth = PlayerHealth.GetComponent<PlayerHealthScript>().currentHealth;
        for (int i = 0; i < heartFills.Length -1; i++)
        {
            if (i < currHealth)
            {
                heartFills[i].fillAmount = 1;
            }
            else
            {
                heartFills[i].fillAmount = 0;
            }
        }
        if (currHealth % 1 != 0)
        {
            int lastPos = Mathf.FloorToInt(currHealth);
            heartFills[lastPos].fillAmount = currHealth % 1;
        }
    }
}
