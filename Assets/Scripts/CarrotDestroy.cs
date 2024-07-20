using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CarrotDestroy : MonoBehaviour
{
    //Usuwanie marchewki
    //Marchewka spawnuje sie W graczu, moze zostac usunieta dopiero po wyjsciu z jego collidera
    //Gdy gracz jest na lewej scianie marchewka spawnuje sie rowniez w niej, dlatego ontriggerstay tez jest wywolywany

    public float carrotDamage = 1f;

    private Rigidbody2D rb;
    private RaycastHit2D[] result = new RaycastHit2D[1];

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        //cast poniewaz czasem marchewka przechodzi przez armor i niszczy mob2 od gory
        rb.Cast(rb.velocity.normalized, result, rb.velocity.magnitude * Time.fixedDeltaTime);
        if (result[0])
        {
            if (result[0].collider.gameObject.tag == "Armor") 
                Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player") return;
        if (collision.tag == "HitPoint")
        {
            //niszczenie hitpointow miniboss2 i inkrementacja jego zmiennej
            Destroy(collision.gameObject);
            collision.gameObject.transform.parent.GetComponent<MBoss2Controller>().hitPointsDestroyed++;
        }
        Destroy(gameObject);
    }
}
