using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Public Properties
    [Header("Player Properties")]
    public float maxHealth = 100f, moveSpeed = 5f;
    public static float health = 100f;
    [Header("Jump Properties")]
    public LayerMask groundMask;
    public Transform groundCheckObject;
    public int maxExtraJumps = 1;
    public float jumpPower = 15f, jumpCooldown = 0.1f, fallMultiplier = 2.25f, lowJumpMultiplier = 1.5f, groundCheckRadius = 0.25f;
    [Header("Dash Properties")]
    public float dashSpeed = 10f, dashCooldown = 0.8f;
    [Header("Time Properties")]
    public float maxTimeLimit = 4f, slowMotionSpeed = 1.25f;
    #endregion

    #region Private Properties
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private Vector2 mouseDirection;
    private int remainingExtraJumps = 1;
    private float inputX, rewindTime = 0f, slowMotionTime = 0f;
    private bool touchingGround = false, isJumping = false;
    #endregion

    #region Currently unused properties
    //[Header("Teleport Dash Properties")]
    //public float teleportMaxDistance = 10f;
    //public float teleportDamage = 20f;
    //public float teleportCooldown = 1.5f;
    //private bool isTeleportingOrDashing = false;
    #endregion

    #region Main code
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        remainingExtraJumps = maxExtraJumps;
        isJumping = false;
        //isTeleportingOrDashing = false;
        rewindTime = maxTimeLimit + 1;
        health = maxHealth;
    }

    void Update()
    {
	    inputX = Input.GetAxisRaw("Horizontal");
        animator.SetFloat("PlayerSpeed", Mathf.Abs(inputX));
	    mouseDirection = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (!TimeManager.Rewinding && Input.GetKeyDown(KeyCode.Return) && rewindTime >= maxTimeLimit + 1)
        {
            TimeManager.RewindTime();
            rewindTime = 0;
        }
        else if (TimeManager.Rewinding && (Input.GetKeyUp(KeyCode.Return) || rewindTime >= maxTimeLimit))
        {
            TimeManager.StopRewinding();
        }
        else if (!TimeManager.Rewinding && Input.GetKeyDown(KeyCode.LeftShift) && slowMotionTime >= maxTimeLimit + 1)
        {
            TimeManager.ChangeTimeFlow(rb, slowMotionSpeed);
        }
        else if (!TimeManager.Rewinding && Input.GetKeyUp(KeyCode.LeftShift) && slowMotionTime >= maxTimeLimit)
        {
            TimeManager.ChangeTimeFlow(rb, 0.25f);
        }
        rewindTime += Time.deltaTime;
        slowMotionTime += Time.deltaTime;
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(inputX * moveSpeed, rb.velocity.y);
        spriteRenderer.flipX = inputX == 0 ? spriteRenderer.flipX : inputX == -1;
        touchingGround = Physics2D.OverlapCircle(groundCheckObject.position, groundCheckRadius, groundMask);
        remainingExtraJumps = touchingGround ? maxExtraJumps : remainingExtraJumps;
        rb.gravityScale = touchingGround ? 0 : 3;

        #region Code that I may use in the future
        //transform.localScale = new Vector3(inputX == 0 ? transform.localScale.x : inputX, transform.localScale.y, 0);

        /*if (Input.GetKeyDown(KeyCode.Q) && !isTeleportingOrDashing)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            rb.gravityScale = 0.5f;
            transform.position = Vector3.MoveTowards(transform.position, mousePos, 10); //* Time.fixedDeltaTime and then increase dashSpeed prob
            isTeleportingOrDashing = true;
            StartCoroutine(teleportDashWait());
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift) && !isTeleportingOrDashing)
        {
            
        }
        */
        #endregion

        if (Input.GetButtonDown("Jump") && !isJumping && remainingExtraJumps > 0)
        {
            animator.SetBool("Jumping", true);
            rb.velocity = new Vector2(rb.velocity.x, 0) + Vector2.up * jumpPower;
            isJumping = true;
            remainingExtraJumps--;
            StartCoroutine(jumpWait());
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity -= Vector2.down * Physics2D.gravity.y * lowJumpMultiplier * Time.fixedDeltaTime;
        }
        else if (rb.velocity.y < 0 && !touchingGround)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * fallMultiplier * Time.fixedDeltaTime;
            animator.SetBool("Jumping", false);
            animator.SetBool("Falling", true);
        }
        else if((rb.velocity.y <= 2 || touchingGround) && animator.GetBool("Falling"))
        {
            animator.SetBool("Falling", false);
        }
    }

    public static float DamagePlayer(float damage)
    {
        Debug.Log($"Player took {System.Math.Round((decimal)damage, 2)} damage!");
        health -= damage;
        return health;
    }

    public IEnumerator jumpWait()
    {
        yield return new WaitForSeconds(jumpCooldown);
        isJumping = false;
    }
    #endregion

    #region More code I may use in the future
    /*public IEnumerator dashWait()
    {
        yield return new WaitForSeconds(dashCooldown);
        rb.gravityScale = 3;
        isTeleportingOrDashing = false;
    }

    public IEnumerator teleportDashWait()
    {
        yield return new WaitForSeconds(teleportCooldown);
        rb.gravityScale = 3;
        isTeleportingOrDashing = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Enemy enemyHit = collision.collider.GetComponent<Enemy>();
        if(isTeleportingOrDashing && enemyHit)
        {
            enemyHit.TakeDamage(dashDamage);
        }
    }

    Vector2 mouseDirection = (mousePos - new Vector2(transform.position.x, transform.position.y)).normalized;
    rb.AddRelativeForce(Quaternion.AngleAxis(Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg, Vector3.forward) * Vector3.right * dashSpeed, ForceMode2D.Impulse);

    Debug.Log("Moving");
    Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    Vector2 mouseDirection = (mousePos - new Vector2(transform.position.x, transform.position.y)).normalized;
    rb.AddRelativeForce(Quaternion.AngleAxis(Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg, Vector3.forward) * Vector3.right * dashSpeed, ForceMode2D.Impulse);
    */
    #endregion
}