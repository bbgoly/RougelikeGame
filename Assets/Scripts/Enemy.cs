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
    public float attackCooldown = 0.5f;
    public float enemyDamage = 10;
    public GameObject deathEffect;
    
    private GameObject player;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

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
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
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
        if (canAttack && !animator.GetBool("Attacking"))
        {
            Player.DamagePlayer(enemyDamage);
            animator.SetBool("Attacking", true);
            StartCoroutine(AttackCoroutine());
        }
    }

    private IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(1.5f + attackCooldown);
        Debug.Log("uwu");
        animator.SetBool("Attacking", false);
    }

    private void FixedUpdate()
    {
        if (!TimeManager.Rewinding && !inRange && !canAttack)
        {
            transform.position = new Vector3(transform.position.x + Mathf.Sin(Time.fixedTime) / 8, transform.position.y);
        }
        else if (!TimeManager.Rewinding && inRange && !canAttack)
        {
            transform.position = new Vector3(Vector3.MoveTowards(transform.position, player.transform.position, 0.1f).x, transform.position.y);
        }
        animator.SetFloat("EnemySpeed", !animator.GetBool("Attacking") ? 1 : 0);
        spriteRenderer.flipX = (player.transform.position.x - transform.position.x) < 0;
    }
}
