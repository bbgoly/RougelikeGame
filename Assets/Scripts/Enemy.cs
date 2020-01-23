using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    #region Public & Protected Fields & Properties
    [Header("Enemy Properties")]
    public string enemyName;
    public float maxHealth, currentHealth, aggroRange, attackRange, attackCooldown, enemyDamage, enemyWalkSpeed;

    protected Transform player;
    protected Animator animator;
    protected Rigidbody2D rb2d;

    protected bool inRange
    {
        get
        {
            return Vector2.Distance(transform.position, player.position) < aggroRange;
        }
    }

    protected bool canAttack
    {
        get
        {
            return inRange && Vector2.Distance(transform.position, player.position) < attackRange;
        }
    }
    #endregion

    #region Private Fields
    private SpriteRenderer spriteRenderer;
    private float attackTime;
    #endregion

    #region Main enemy code
    private void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player>().transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!TimeManager.Rewinding && !animator.GetBool("EnemyMoving") && attackTime >= attackCooldown && canAttack)
        {
            Attack();
            attackTime = 0;
        }
        attackTime += Time.deltaTime;
    }

    protected virtual void FixedUpdate()
    {
        if (!TimeManager.Rewinding)
        {
            Vector2 playerDirection = transform.position - player.position;
            float rotationAngle = Mathf.Atan2(playerDirection.y, playerDirection.x) * Mathf.Rad2Deg + 180f;
            spriteRenderer.flipX = !inRange && Mathf.Sin(Time.fixedTime) < 0;
            spriteRenderer.flipY = inRange && rotationAngle > 90 && rotationAngle < 270;
            rb2d.rotation = inRange ? rotationAngle : 0;
            animator.SetBool("EnemyMoving", !canAttack);
        }
    }

    public abstract void Attack();

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= Mathf.Min(damage, currentHealth);
        Debug.Log($"{enemyName} took {System.Math.Round((decimal)damage, 2)} damage!");
        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }
    #endregion
}