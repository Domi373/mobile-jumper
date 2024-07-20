using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mob4BulletController : MonoBehaviour
{
    public int bulletMaxBounce = 3;

    private bool isMBoss = false;
    private Rigidbody2D rb;
    private BossController bossController;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "StillRock(Clone)") return;
        if (collision.gameObject.name == "Mob4Bullet(Clone)") return;
        if (collision.gameObject.name == "Mob4(Clone)") return;
        if (collision.gameObject.name == "MBoss4(Clone)")
        {
            isMBoss = true;
            return;
        }
        if (collision.gameObject.name == "Boss(Clone)")
        {
            if (bossController = collision.gameObject.GetComponent<BossController>())
                if (bossController.isBulletBounce)
                    isMBoss = true;
            return;
        }

        if ((collision.gameObject.name == "Left wall" || collision.gameObject.name == "Right wall") && isMBoss)
        {
            if (bulletMaxBounce <= 0)
                Destroy(gameObject);

            rb.velocity = new Vector2(rb.velocity.x * -1, rb.velocity.y);
            bulletMaxBounce--;
            return;
        }

        if ((collision.gameObject.name == "Floor" || collision.gameObject.name == "Roof") && isMBoss)
        {
            if (bulletMaxBounce <= 0)
                Destroy(gameObject);

            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * -1);
            bulletMaxBounce--;
            return;
        }


        Destroy(gameObject);
    }
}
