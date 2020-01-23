using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public static Player player;
    public Enemy enemy;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player hitPlayer = collision.GetComponent<Player>();
        Enemy hitEnemy = collision.GetComponent<Enemy>();
        print(hitEnemy);
        if (!enemy && hitEnemy)
        {
            hitEnemy.TakeDamage(player.attackDamage);
        }
        else if (enemy && hitPlayer)
        {
            player.DamagePlayer(enemy);
        }


        if (collision.gameObject.layer != 0 && (!enemy && collision.gameObject.layer != 10 || enemy && collision.gameObject.layer != 9))
        {
            Destroy(gameObject);
        }
    }
}
