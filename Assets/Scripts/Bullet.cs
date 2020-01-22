using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Enemy enemy;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player)
        {
            Player.DamagePlayer(enemy);
        }

        if (collision.gameObject.layer != 0 && collision.gameObject.layer != 9)
        {
            Destroy(gameObject);
        }
    }
}
