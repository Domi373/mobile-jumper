using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : MonoBehaviour
{
    public float bombTimer = 3f;

    [HideInInspector]
    public float bombScale;

    private float time = 0f;
    private bool explode = true;

    void Update()
    {
        time += Time.deltaTime;
        if (time >= bombTimer && explode)
        {
            explode = false;

            //na poczatku bomba nie jest triggerem ale posiada rb wiec posiada kolizje, dlatego usuwamy stary collider
            Destroy(gameObject.GetComponent<CircleCollider2D>());
            //dodajemy nowy collider, ktory jest triggerem i zmieniamy tag na mob, aby zadzialal triggerEnter playera znowu
            CircleCollider2D cc = gameObject.AddComponent<CircleCollider2D>();
            cc.isTrigger = true;
            tag = "Mob";
            transform.localScale = new Vector3(2f, 2f, 2f) + new Vector3(bombScale, bombScale, bombScale);
            cc.radius = 0.5f;
        }

        if (time >= bombTimer + 0.05f)
            Destroy(gameObject);
    }
}
