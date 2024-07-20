using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class MBoss5Controller : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float rotationSpeed = 1f;
    public float bombDistanceFromMob = 1f;
    public GameObject bomb;
    public float score = 100f;
    public float bombCD = 2f;
    public GameObject indicator;

    private Rigidbody2D rb;
    private GameObject player;
    private GameObject bombClone;
    private float bombCDTimer = 0f;
    private GameObject clone;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (GameObject.FindGameObjectWithTag("Player"))
            player = GameObject.FindGameObjectWithTag("Player");

        bombCD += bomb.GetComponent<BombController>().bombTimer;
        bombCDTimer = bombCD;
    }

    private void Update()
    {
        bombCDTimer += Time.deltaTime;
        if (bombCDTimer >= bombCD)
        {
            bombClone = Instantiate(bomb, transform.position + transform.forward, Quaternion.identity);
            bombCDTimer = 0;
        }

        //spawn indykatora kiedy mob jest poza ekranem
        RaycastHit2D hit = Physics2D.Raycast(player.transform.position, (transform.position - player.transform.position).normalized, (transform.position - player.transform.position).magnitude);
        if (hit.collider != null && hit.transform.tag == "Wall" && rb.velocity != Vector2.zero && !clone)
        {
            clone = Instantiate(indicator, hit.point, Quaternion.identity);
        }
        else if (hit.collider != null && hit.transform.tag == "Wall" && rb.velocity != Vector2.zero && clone)
        {
            clone.transform.position = hit.point;
        }
        else
        {
            Destroy(clone);
        }
    }

    private void FixedUpdate()
    {
        //ruch, kierunek ruchu
        rb.velocity = transform.up * moveSpeed;

        Quaternion targetRotation = Quaternion.LookRotation(transform.forward, player.transform.position - transform.position);
        Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed);

        transform.rotation = rotation;

        //pozycja bomby przed mobem
        if (bombClone)
            bombClone.transform.position = transform.position + transform.up * bombDistanceFromMob;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Carrot")
        {
            if (bombClone)
                bombClone.GetComponent<Rigidbody2D>().velocity = transform.right * -1 * Vector3.Cross(transform.up, player.transform.position - transform.position).normalized.z;

            Destroy(gameObject);
            if (GameObject.Find("GameController").GetComponent<GameController>())
                GameObject.Find("GameController").GetComponent<GameController>().gameScore += score;
        }
    }
}
