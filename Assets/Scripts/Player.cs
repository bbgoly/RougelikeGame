using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Public Fields
    [Header("Player Properties")]
    public GameObject ghostEffect;
    public float maxHealth = 100f, moveSpeed = 5f;
    public static bool stunned = false;
    [Header("Dash Properties")]
    public float dashMultiplier = 2f;
    public float dashCooldown = 0.4f, ghostLimit = 1f;
    [Header("Time Properties")]
    public float slowMotionCooldown = 2f;
    public float slowMotionSpeed = 0.3f, rewindSpeed = 3f;
    #endregion

    #region Private Fields
    private Rigidbody2D rb2d;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private float inputX, inputY, dashTime, slowMotionTime;
    private static float health = 100f;
    private bool dashing = false;
    #endregion

    #region Main Code
    private void Start()
    {
        health = maxHealth;
        dashTime = dashCooldown;
        slowMotionTime = slowMotionCooldown;
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary> 
    /// Update method used to get input, and clone effects
    /// <para> 
    /// animator.SetFloat(...): Basically sets a float to the PlayerSpeed animation parameter. If the parameter is greater than 0.1f then the animation will play. 
    /// Since InputX and InputY's value work in the same sense as a cartesian plane (left is -1 for inputX, up is 1 for inputY), I decided to add both their absolute values together </para>
    /// <para> 
    /// dashTime: If the player presses the LeftShift key, and the time since the last dash (dashTime) is greater than the dashCooldown value then reset the dashTime variable,
    /// else add onto the dashTime variable using the time in seconds since the last frame (Time.deltaTime)
    /// </para>
    /// <para>
    /// dashing: If dashTime is less than the dashCooldown subtracted by 0.4f, then end the dash animation. The reason why I subtract by 0.4f is to have the dash last a portion of the cooldown, and then
    /// use the remaining time to act as a cooldown.
    /// </para>
    /// <para>
    /// if statement: If the player is dashing, or the player is in slow motion and the dashTime or slowMotionTime is within the portion of the cooldown where the player can dash or be in slow motion,
    /// then call the CreateGhosts method <see cref="CreateGhosts"/>.
    /// if statement 2: The reason why I create a new if statement with the same conditions without the CreateGhosts method is so I don't have any weird conflicts where the player will have two times the
    /// ghosts spawning if the player is BOTH dashing and in slow motion (because I was originally going to have two seperate if statements both with the CreateGhosts method, but I figured it would
    /// be easier this way). Also, I don't like nested if statements lul. Inside of the if statement, a method from the TimeManager script is called to lower the timeScale property of the built-in
    /// Time class to create a slow motion effect.
    /// </para>
    /// </summary>
    private void Update()
    {
        float dashEndTime = dashCooldown - 0.2f;
        float slowMotionEndTime = slowMotionCooldown - 1.2f;

        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");
        animator.SetFloat("PlayerSpeed", Mathf.Abs(inputX) + Mathf.Abs(inputY));
        animator.SetBool("PlayerAbilityReady", dashTime > dashCooldown || slowMotionTime > slowMotionCooldown);

        dashTime += !TimeManager.Rewinding && Input.GetKeyDown(KeyCode.LeftShift) && dashTime > dashCooldown ? -dashTime : Time.deltaTime;
        dashing = !TimeManager.Rewinding && dashTime < Mathf.Abs(dashCooldown - dashEndTime);
        slowMotionTime += Time.deltaTime;

        //TimeManager.SlowMo = !TimeManager.Rewinding && Input.GetKey(KeyCode.Q) && slowMotionTime > Mathf.Abs(slowMotionCooldown);
        //Time.timeScale = TimeManager.SlowMo && slowMotionTime < slowMotionCooldown - 1.2f ? slowMotionSpeed : (TimeManager.Rewinding ? rewindSpeed : 1);
        //slowMotionTime += TimeManager.SlowMo && slowMotionTime > slowMotionCooldown ? -slowMotionTime : Time.deltaTime;

        if (dashing && dashTime < dashCooldown - dashEndTime || TimeManager.SlowMo && slowMotionTime < slowMotionCooldown - slowMotionEndTime)
        {
            CreateGhosts();
        }

        if (!TimeManager.Rewinding && Input.GetKeyDown(KeyCode.Q) && slowMotionTime > slowMotionCooldown)
        {
            TimeManager.SlowMo = true;
            Time.timeScale = slowMotionSpeed;
            rb2d.interpolation = RigidbodyInterpolation2D.Interpolate;
            slowMotionTime = 0;
        }
        else if (TimeManager.SlowMo && (Input.GetKeyUp(KeyCode.Q) || TimeManager.Rewinding || slowMotionTime > slowMotionCooldown - slowMotionEndTime))
        {
            TimeManager.SlowMo = false;
            Time.timeScale = TimeManager.Rewinding ? rewindSpeed : 1;
            rb2d.interpolation = RigidbodyInterpolation2D.None;
        }
        print(animator.GetBool("PlayerAbilityReady"));
    }

    /// <summary> 
    /// FixedUpdate method to handle everything physics related
    /// <para> 
    /// mouseDirection: Creates a new Vector2 that points in the direction of the mouse's world space position </para>
    /// <para>
    /// rotationAngle: Gets the angle from the player's position to the mouse's world space position, converts it into degrees (since Mathf.Atan2 returns radians, and Rigidbody2D's rotation property
    /// only accepts degrees), and flips it by 180 degrees to have the player correctly face the mouse.
    /// </para>
    /// <para>
    /// velocity: If the player is dashing, then set the velocity of the player's Rigidbody2D to the negative mouseDirection (see above), and multiplies it by a scalar to increase the velocity (this
    /// line is what makes the player dash towards the mouse's direction). If the player is not dashing then create a new Vector2 with the inputX and inputY values multiplied by the moveSpeed scalar.
    /// </para>
    /// <para>
    /// flipY: Flips the player's sprite on the Y axis if the player were to rotate between two specific angles to keep it looking nice
    /// </para>
    /// <para>
    /// rotation: Sets the rotation of the player's Rigidbody2D component to the rotationAngle variable (see above)
    /// </para>
    /// </summary>
    private void FixedUpdate()
    {
        Vector2 mouseDirection = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float rotationAngle = Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg + 180f;
        rb2d.velocity = stunned ? Vector2.zero : dashing ? -mouseDirection * dashMultiplier : new Vector2(inputX, inputY) * moveSpeed;
        spriteRenderer.flipY = rotationAngle > 90 && rotationAngle < 270;
        rb2d.rotation = rotationAngle;
    }

    /// <summary>
    /// Method that handles the creation of the ghost trail effect
    /// <para>
    /// If the player is dashing or in slow motion, and the dashTime or slowMotionTime is within the portion of the cooldown where the player can dash, then create a ghost trail effect to have 
    /// multiple player copies follow the player. This is done by instantiating a ghost prefab at the player's position and rotation with a fade animation attached to it's animator component that plays
    /// on entry (once the prefab is added to the game). The current sprite of the ghost prefab is set to the current sprite of the player so the ghost can replicate the player's past movements, and 
    /// the flipY of the ghost's SpriteRenderer component is set to the player's flipY so the ghost's sprite can flip along with the player if the mouse is on the left of the player. Lastly, the ghost 
    /// prefab is destroyed after a certain time limit.
    /// </para>
    /// </summary>
    private void CreateGhosts()
    {
        GameObject ghost = Instantiate(ghostEffect, transform.position, transform.rotation);
        SpriteRenderer ghostSpriteRenderer = ghost.GetComponent<SpriteRenderer>();
        ghostSpriteRenderer.sprite = spriteRenderer.sprite;
        ghostSpriteRenderer.flipY = spriteRenderer.flipY;
        Destroy(ghost, ghostLimit);
    }

    public static void DamagePlayer(Enemy enemy)
    {
        float enemyDamage = enemy.enemyDamage + Random.Range(-2, 2);
        health -= Mathf.Min(enemyDamage, health);
        //Debug.Log($"Player took {System.Math.Round((decimal)enemyDamage, 2)} damage from {enemy.enemyName}! Player has {health} health remaining!");
        if (health <= 0)
        {
            //Debug.Log($"Player died to {enemy.enemyName}");
            //Instantiate(deathEffect, FindObjectOfType<Player>().transform.position, Quaternion.identity);
            //Destroy(gameObject);
        }
    }
    #endregion
}