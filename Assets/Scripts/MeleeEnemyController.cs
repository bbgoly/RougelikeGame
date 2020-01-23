using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeEnemyController : Enemy
{
    [Header("Charge Properties")]
    public float chargeSpeed = 4f;

    private bool charging = false;
    
    public override void Attack()
    {
        StartCoroutine(ChargeCooldown());
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (!TimeManager.Rewinding)
        {
            Vector2 playerDirection = transform.position - player.position;
            rb2d.velocity = !canAttack && inRange ? -playerDirection * enemyWalkSpeed : canAttack ? rb2d.velocity : new Vector2(Mathf.Sin(Time.fixedTime) * 2, 0);
            rb2d.rotation = charging ? Mathf.Atan(playerDirection.y / playerDirection.x) * Mathf.Rad2Deg : rb2d.rotation;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (charging && player)
        {
            player.DamagePlayer(this);
            player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

    private IEnumerator ChargeCooldown()
    {
        charging = true;
        rb2d.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.4f);
        rb2d.velocity = -(transform.position - player.position) * chargeSpeed;
        yield return new WaitForSeconds(0.8f);
        rb2d.velocity = Vector2.zero;
        charging = false;
    }
}
