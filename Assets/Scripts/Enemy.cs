using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    #region Public Properties
    public string enemyName;
    public float maxHealth, currentHealth, aggroRange, attackRange, attackCooldown, enemyDamage, enemyWalkSpeed;

    public Transform player;
    public Animator animator;
    public BoxCollider2D c2d;
    public GameObject deathEffect;

    public bool inRange
    {
        get
        {
            return Vector2.Distance(transform.position, player.position) < aggroRange;
        }
    }

    public bool canAttack
    {
        get
        {
            return Vector2.Distance(transform.position, player.position) < attackRange;
        }
    }
    #endregion

    #region Private Properties
    private Vector3 currentVelocity;
    private SpriteRenderer spriteRenderer;
    #endregion

    #region Main enemy code
    private void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        c2d = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void Attack()
    {
        animator.SetFloat("WalkSpeed", 0.1f);
        StartCoroutine(AttackCoroutine());
    }

    public virtual void TakeDamage(float damage)
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

    public virtual IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(1.5f + attackCooldown);
    }

    private void FixedUpdate()
    {
        if (!TimeManager.Rewinding && !inRange && !canAttack)
        {
            transform.position = Vector3.SmoothDamp(transform.position, new Vector3(transform.position.x + Mathf.Sin(Time.fixedTime), transform.position.y), ref currentVelocity, enemyWalkSpeed + 0.3f);
            animator.SetFloat("WalkSpeed", 0.5f);
        }
        else if (!TimeManager.Rewinding && inRange && !canAttack)
        {
            transform.position = Vector3.SmoothDamp(transform.position, new Vector3(Vector3.MoveTowards(transform.position, player.position, enemyWalkSpeed).x, transform.position.y), ref currentVelocity, 0.05f); 
            animator.SetFloat("WalkSpeed", 1);
        }
        spriteRenderer.flipX = (inRange && (player.position.x - transform.position.x) < 0) || Mathf.Sin(Time.fixedTime) < 0;
    }

    private void Update()
    {
        if (canAttack && !animator.GetBool("Attacking"))
        {
            Attack();
        }
    }
    #endregion
}