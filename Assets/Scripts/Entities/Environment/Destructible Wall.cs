using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DestructibleWall : MonoBehaviour
{
    [Header("Health")]
    public float currentHealth;
    private float maxHealth = 5;
    private float maxPossibleHealth = 16;

    private float flashTime = .2f;
    [Header("Components")]
    public Tilemap tilemap;
    public GameObject otherObjectToKill;
    private void Awake()
    {
        currentHealth = maxHealth;
        tilemap = GetComponent<Tilemap>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 8)
        {
            takeDamage(1);
        }
    }
    public void takeDamage(float Damage)
    {
        loseHealth(Damage);
    }
    private void loseHealth(float Damage)
    {
        currentHealth -= Damage;
        if (currentHealth <= 0)
        {
            StartCoroutine(DestroyWall());
        }
        else
        {
            StartCoroutine(FlashRed());
        }
    }
    public IEnumerator DestroyWall()
    {
        yield return new WaitForSeconds(.05f);
        if(otherObjectToKill != null) otherObjectToKill.SetActive(false);
        gameObject.SetActive(false);
    }
    public IEnumerator FlashRed()
    {
        tilemap.color = Color.red;
        yield return new WaitForSeconds(flashTime);
        tilemap.color = Color.white;
    }
}
