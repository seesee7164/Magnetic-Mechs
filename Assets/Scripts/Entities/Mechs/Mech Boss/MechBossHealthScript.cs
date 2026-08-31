using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MechBossHealthScript : MonoBehaviour
{
    public Image remainingHealth;
    //script for managing the players health
    [Header("Components")]
    public GameObject Mech;
    private MechActionsScript mechActionsScript;
    [Header("Health")]
    public float currentHealth;
    private float maxHealth = 30;
    [Header("Invincibility")]
    const float invincibilityTimeDefault = .5f;
    public bool invincible;
    private void Awake()
    {
        currentHealth = maxHealth;
        invincible = false;
        if (Mech != null) mechActionsScript = Mech.GetComponent<MechActionsScript>();
    }
    public void takeDamage(float Damage, Vector2 knockbackDirection, float knockback = 0, float invincibilityTime = invincibilityTimeDefault, bool DeathPit = false)
    {
        if (!invincible)
        {
            StartCoroutine(handleDamage(Damage, invincibilityTime, DeathPit));
            if (DeathPit && Damage >= currentHealth) return;
            StartCoroutine(mechActionsScript.handleKnockback(knockback, knockbackDirection));
        }
    }
    IEnumerator handleDamage(float Damage, float invincibilityTime, bool DeathPit)
    {
        invincible = true;
        loseHealth(Damage, DeathPit);
        yield return new WaitForSeconds(invincibilityTime);
        invincible = false;
    }
    private void loseHealth(float Damage, bool DeathPit)
    {
        currentHealth -= Damage;
        if (currentHealth <= 0)
        {
            KillBoss(DeathPit);
        }
        updateHealthBar();
    }
    private void updateHealthBar()
    {
        remainingHealth.fillAmount = currentHealth / maxHealth;
    }
    public void KillBoss(bool DeathPit = false)
    {
        currentHealth = 0;
        mechActionsScript.KillPlayer(DeathPit);
        Debug.Log("Fill out Mech boss death behavior");
    }
    public float getMaxHealth()
    {
        return maxHealth;
    }
}
