﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyController : Enemy
{
    [Header("Bullet Properties")]
    public int maxBullets;
    public float bulletSpeed;
    public GameObject bulletPrefab;

    #region Ranged enemy code
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (!TimeManager.Rewinding)
        {
            Vector2 playerDirection = transform.position - player.position;
            rb2d.velocity = !canAttack && inRange ? -playerDirection * enemyWalkSpeed : canAttack ? Vector2.zero : new Vector2(Mathf.Sin(Time.fixedTime) * 2, 0);
        }
    }

    public override void Attack()
    {
        StartCoroutine(GenerateBullets());
    }

    private IEnumerator GenerateBullets()
    {
        for (int i = 0; i < maxBullets; i++)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
            bullet.GetComponent<Rigidbody2D>().velocity = transform.right * bulletSpeed;
            bullet.GetComponent<Bullet>().enemy = this;
            Destroy(bullet, 3f);
            yield return new WaitForSeconds(0.3f);
        }
    }
    #endregion
}
