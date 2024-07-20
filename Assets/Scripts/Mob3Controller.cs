using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mob3Controller : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float rotationSpeed = 1f;
    public float score = 100f;
    public GameObject indicator;

    private Rigidbody2D rb;
    private GameObject player;
    private GameObject clone;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (GameObject.FindGameObjectWithTag("Player"))
            player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
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
        Quaternion targetRotation = Quaternion.LookRotation(transform.forward, player.transform.position - transform.position);
        Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed);
        transform.rotation = rotation;
        rb.velocity = transform.up * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Carrot")
        {
            Destroy(gameObject);
            if (GameObject.Find("GameController").GetComponent<GameController>())
                GameObject.Find("GameController").GetComponent<GameController>().gameScore += score;
        }
    }
}
