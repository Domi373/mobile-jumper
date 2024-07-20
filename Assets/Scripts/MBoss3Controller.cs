using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public class MBoss3Controller : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float score = 100f;
    public GameObject indicator;
    public float distanceFromPlayer = 1f;
    public float dashSpeed = 1f;
    public float rotationSpeed = 3.5f;
    public float dashLerpSpeed = 0.5f;
    public float dashDistance = 3f;

    private Rigidbody2D rb;
    private GameObject player;
    private GameObject clone;
    private bool canTakeDMG = false;
    private Vector3 dashStartPosition;
    private bool isDashing = false;

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
        //dash kiedy jest w zasiegu gracza i porusza sie z domyslna predkoscia
        if ((player.transform.position - transform.position).magnitude <= distanceFromPlayer && rb.velocity.magnitude <= moveSpeed + 0.1f && !isDashing)
        {
            isDashing = true;
            canTakeDMG = true;
            rb.velocity = transform.up * dashSpeed;
            GetComponent<SpriteRenderer>().color = new Color(0.1792453f, 0.06172125f, 0.06172125f, 1f);
            dashStartPosition = transform.position;
        }
        else if ((dashStartPosition - transform.position).magnitude > dashDistance || !isDashing)
        {
            canTakeDMG = false;
            isDashing = false;

            //rotacja w strone gracza
            Quaternion targetRotation = Quaternion.LookRotation(transform.forward, player.transform.position - transform.position);
            Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed);
            transform.rotation = rotation;

            //lerp prêdkoœci
            float curMoveSpeed = Mathf.Lerp(rb.velocity.magnitude, moveSpeed, 1 - Mathf.Pow(0.5f, Time.fixedDeltaTime * dashLerpSpeed));
            rb.velocity = transform.up * curMoveSpeed;
            GetComponent<SpriteRenderer>().color = new Color(0.1792453f, 0.06172125f, 0.06172125f, 0.5f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Carrot" && canTakeDMG)
        {
            Destroy(gameObject);
            if (GameObject.Find("GameController").GetComponent<GameController>())
                GameObject.Find("GameController").GetComponent<GameController>().gameScore += score;
        }
    }
}
