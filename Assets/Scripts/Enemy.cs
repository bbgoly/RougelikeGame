using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public string enemyName;
    public int maxHealth = 100;
    public float currentHealth = 100;
    public float aggroRange = 20;
    public float attackRange = 5;
    public float enemyDamage = 10;
    public GameObject deathEffect;
    
    private GameObject player;

    private bool inRange
    {
        get
        {
            return Vector2.Distance(transform.position, player.transform.position) < aggroRange;
        }
    }

    private bool canAttack
    {
        get
        {
            return Vector2.Distance(transform.position, player.transform.position) < attackRange;
        }
    }

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= Mathf.Min(damage, currentHealth);
        Debug.Log($"{enemyName} took {System.Math.Round((decimal)damage, 2)} damage!");
        if (currentHealth <= 0)
        {
            Debug.LogWarning($"{enemyName} died!");
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (canAttack)
        {
            Player.DamagePlayer(enemyDamage);
        }
    }


    private void FixedUpdate()
    {
        if (!TimeManager.Rewinding && !inRange)
        {
            transform.position = new Vector3(transform.position.x + Mathf.Sin(Time.fixedTime) / 8, transform.position.y);
        }
        else if (inRange && !canAttack)
        {
            transform.position = new Vector3(Vector3.MoveTowards(transform.position, player.transform.position, 0.1f).x, transform.position.y);
        }
        Debug.Log(inRange);
    }
}
