using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Mob1Controller : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float score = 100f;
    public GameObject indicator;

    [HideInInspector]
    public Vector2 moveDirection;

    private Rigidbody2D rb;
    private bool canChangeDirection = false;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        rb.velocity = moveDirection * moveSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Carrot")
        {
            Destroy(gameObject);
            if (GameObject.Find("GameController").GetComponent<GameController>())
                GameObject.Find("GameController").GetComponent<GameController>().gameScore += score;
        }

        if ((collision.tag == "Wall" || (collision.tag == "Player" && collision.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude == 0f)) && canChangeDirection)
            moveDirection *= -1;

        //spawn indykatora
        if (collision.name == "Right wall" && canChangeDirection == false)
            Instantiate(indicator, new Vector3(collision.transform.position.x - collision.bounds.size.x / 2, transform.position.y, 0f), Quaternion.identity);
        else if (collision.name == "Left wall" && canChangeDirection == false)
            Instantiate(indicator, new Vector3(collision.transform.position.x + collision.bounds.size.x / 2, transform.position.y, 0f), Quaternion.identity);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Wall")
            canChangeDirection = true;
    }
}
