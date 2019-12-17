using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    [Header("Movement Properties")]
    public float moveSpeed = 5f;
    [Space]
    [Header("Dash Properties")]
    public float dashSpeed = 10f;
    public float dashDamage = 15f;
    public float dashCooldown = 0.8f;
    [Space]
    [Header("Teleport Dash Properties")]
    public float teleportMaxDistance = 10f;
    public float teleportDamage = 20f;
    public float teleportCooldown = 1.5f;
    private bool isTeleportingOrDashing = false;
    [Space]
    [Header("Jump Properties")]
    public float jumpForce = 15f;
    public float fallMultiplier = 2.25f;
    public float lowJumpMultiplier = 1.5f;
    [Space]
    public int maxJumps = 2;
    private int remainingJumps = 2;
    public float jumpCooldown = 0.1f;
    private bool isJumping = false;
    [Space]
    public LayerMask groundMask;
    public Transform groundCheckObject;
    public float groundCheckRadius = 0.25f;
    private bool touchingGround = false;
    
    private float inputX;
    private Vector2 mouseDirection;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        remainingJumps = maxJumps;
        isJumping = false;
        isTeleportingOrDashing = false;
    }

    void Update()
    {
	inputX = Input.GetAxisRaw("Horizontal");
	mouseDirection = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
	
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(inputX * moveSpeed, rb.velocity.y);
        transform.localScale = new Vector3(inputX == 0 ? transform.localScale.x : inputX, transform.localScale.y, 0);
        touchingGround = Physics2D.OverlapCircle(groundCheckObject.position, groundCheckRadius, groundMask);
        remainingJumps = touchingGround ? maxJumps : remainingJumps;
        rb.gravityScale = touchingGround ? 0 : 3;

        if (Input.GetKeyDown(KeyCode.Q) && !isTeleportingOrDashing)
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

        if (Input.GetButtonDown("Jump") && !isJumping && remainingJumps > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0) + Vector2.up * jumpForce;
            isJumping = true;
            remainingJumps--;
            StartCoroutine(jumpWait());
        }
        else if (rb.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            rb.velocity -= Vector2.up * Physics2D.gravity.y * lowJumpMultiplier * Time.fixedDeltaTime; //Or Vector2.down
        }
        else if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * fallMultiplier * Time.fixedDeltaTime;
        }
    }

    public IEnumerator jumpWait()
    {
        yield return new WaitForSeconds(jumpCooldown);
        isJumping = false;
    }

    public IEnumerator dashWait()
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

    /*
    Vector2 mouseDirection = (mousePos - new Vector2(transform.position.x, transform.position.y)).normalized;
    rb.AddRelativeForce(Quaternion.AngleAxis(Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg, Vector3.forward) * Vector3.right * dashSpeed, ForceMode2D.Impulse);

    Debug.Log("Moving");
    Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    Vector2 mouseDirection = (mousePos - new Vector2(transform.position.x, transform.position.y)).normalized;
    rb.AddRelativeForce(Quaternion.AngleAxis(Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg, Vector3.forward) * Vector3.right * dashSpeed, ForceMode2D.Impulse);
    */
}
