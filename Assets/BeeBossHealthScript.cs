using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeeBossHealthScript : MonoBehaviour
{
    //manages health for the robot spider queen
    [Header("Components")]
    //public GameObject Player;
    public BeeBossScript beeBossScript;
    public BeeBossParentScript beeBossParentScript;
    public DroneRespawnerScript droneRespawnerScript;
    public Image remainingHealth;
    public LogicScript logic;
    [Header("Health")]
    public float currentHealth;
    private float maxHealth = 30;
    public float maxPossibleHealth = 100;
    [Header("Variables")]
    private bool inStage2 = false;
    private void Awake()
    {
        GameObject beeBoss = GameObject.FindGameObjectWithTag("BeeBoss");
        if (beeBoss != null) beeBossScript = beeBoss.GetComponent<BeeBossScript>();
        else Debug.Log("Bee Boss could not be found");
        GameObject beeBossParent = GameObject.FindGameObjectWithTag("BeeBossParent");
        if (beeBossParent != null) beeBossParentScript = beeBossParent.GetComponent<BeeBossParentScript>();
        else Debug.Log("Bee Boss Parent could not be found");
        currentHealth = maxHealth;
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
        //invincible = false;
    }
    void Start()
    {

    }
    public void takeDamage(float Damage)
    {
        loseHealth(Damage);
        updateHealthBar();
    }
    private void loseHealth(float Damage)
    {
        currentHealth -= Damage;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            HandleBossDeath();
            beeBossParentScript.BossDied();
        }
        else if (!inStage2 && currentHealth <= maxHealth * 2 / 3)
        {
            beeBossParentScript.ActivateStage2();
            droneRespawnerScript.ActivatePhase2();
            inStage2 = true;
        }
    }
    private void updateHealthBar()
    {
        remainingHealth.fillAmount = currentHealth / maxHealth;
    }
    public void HandleBossDeath()
    {
        if (beeBossScript == null) return;
        logic.StartPostBeeBossDelay();
        beeBossScript.KillBoss();
        beeBossParentScript.BossDied();
    }
}
