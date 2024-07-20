using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MBoss4Controller : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float score = 200f;
    public GameObject shootObject;
    public float shootSpeed = 1f;
    public float shootCD = 1f;
    public GameObject indicator;
    public float deathDistanceFromPlayer = 3f;


    private Rigidbody2D rb;
    private GameObject player;
    private Vector2 endPos;
    private bool isReady = false;
    private float shootTimer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (GameObject.FindGameObjectWithTag("Player"))
            player = GameObject.FindGameObjectWithTag("Player");

        shootTimer = shootCD;

        endPos = transform.position;
        endPos.y -= 2f + Random.value;
        rb.velocity = new Vector2(0f, -moveSpeed);
    }

    private void Update()
    {
        if (transform.position.y <= endPos.y)
        {
            rb.velocity = Vector2.zero;
            isReady = true;
        }

        //shoot
        if (isReady && shootTimer >= shootCD)
        {
            Vector2 playerPos = player.transform.position;
            GameObject clone = Instantiate(shootObject, transform.position, Quaternion.identity);

            if (clone.GetComponent<Rigidbody2D>())
            {
                Rigidbody2D clonerb = clone.GetComponent<Rigidbody2D>();
                clonerb.velocity = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y).normalized * shootSpeed;
            }

            shootTimer = 0f;
        }

        if (isReady)
            shootTimer += Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Carrot" && (transform.position - player.transform.position).magnitude >= deathDistanceFromPlayer)
        {
            Destroy(gameObject);
            if (GameObject.Find("GameController").GetComponent<GameController>())
                GameObject.Find("GameController").GetComponent<GameController>().gameScore += score;
        }

        //spawn indykatora
        if (other.tag == "Wall")
            Instantiate(indicator, new Vector3(transform.position.x, other.transform.position.y - other.bounds.size.y / 2, 0), Quaternion.identity);
    }
}
