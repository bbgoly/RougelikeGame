using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AREnemyController : Enemy
{
    #region AR enemy code
    public override void Attack()
    {
        base.Attack();
        animator.SetTrigger("Attacking");
        c2d.size = new Vector2(c2d.size.x, 0.405f);
        RaycastHit2D hit2D = Physics2D.Raycast(transform.position, transform.position - player.position);
        if (hit2D.collider.GetComponent<Player>())
        {
            Player.DamagePlayer(enemyDamage);
        }
    }

    public override IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(attackCooldown);
    }
    #endregion
}
