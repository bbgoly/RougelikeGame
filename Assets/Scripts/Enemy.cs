using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public int maxHealth = 100;
    public float currentHealth;
    public GameObject deathEffect;

    public void TakeDamage(float damage)
    {
        currentHealth -= Mathf.Min(damage, currentHealth);
        Debug.Log($"{enemyName} took {Math.Round((decimal)damage, 2)} damage!");
        if (currentHealth <= 0)
        {
            Debug.LogWarning($"{enemyName} died!");
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        transform.position = new Vector3(Mathf.PingPong(Time.time, 3), transform.position.y);
    }
}
