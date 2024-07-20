using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RabbitController : MonoBehaviour
{
    [Header("Jump")]
    public float jumpForce;
    public float jumpDeathZone = 50f;
    public float maxForce;

    [Space(10)]

    [Header("Knockback")]
    public float knockBackMultiplier = 1f;

    public float immunityCD = 3f;
    public float playerHP = 3f;

    [HideInInspector]
    public bool canTakeDMG = true;



    private Rigidbody2D rb;
    private Vector2 startMousePosition;
    private bool canJump;
    private RaycastHit2D[] result = new RaycastHit2D[1];
    private string standingOn = "";
    private float immunityTimer;

    private bool doJump = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (immunityTimer < immunityCD)
            immunityTimer += Time.deltaTime;
        else
        {
            canTakeDMG = true;
            GetComponent<SpriteRenderer>().color = Color.white;
        }

        if (Input.GetMouseButtonDown(0))
        {
            startMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (startMousePosition != new Vector2(Input.mousePosition.x, Input.mousePosition.y))
                doJump = true;
        }
    }

    private void FixedUpdate()
    {
        if (doJump)
            Jump();

        // odbicie od moba jako cast poniewaz gracz przy duzej predkosci przykleja sie do mob2 od dolu a powinien sie odbic
        rb.Cast(rb.velocity.normalized, result, rb.velocity.magnitude * Time.fixedDeltaTime);
        if (result[0])
        {
            // cast nie zawsze lapie mob4bullet wiec otrzymanie obrazen przeniesione do triggerenter
            if (result[0].collider.name == "Mob4Bullet(Clone)") return;
            if (result[0].collider.name == "Rock(Clone)") return;

            if ((result[0].collider.tag == "Mob" || result[0].collider.tag == "HitPoint") && canTakeDMG)
            {
                Vector2 knockBackDir = -rb.velocity.normalized;
                rb.velocity = Vector2.zero;
                rb.AddForce(knockBackDir * knockBackMultiplier, ForceMode2D.Impulse);

                if (playerHP > 0)
                    TakeDMG();
            }

            result = new RaycastHit2D[1];
        }
    }

    private void Jump()
    {
        doJump = false;

        Vector2 endMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 jumpDirection = endMousePosition - startMousePosition;

        //kierunek w ktorym MUSI skoczyc krolik np. jesli jest na lewej scianie to nie moze sie po niej slizgac tylko musi odskoczyc
        if (standingOn == "Floor" && jumpDirection.y <= 0f) return;
        else if (standingOn == "Left wall" && jumpDirection.x <= 0f) return;
        else if (standingOn == "Right wall" && jumpDirection.x  >= 0f) return;
        else if (standingOn == "Roof" && jumpDirection.y  >= 0f) return;

        if (jumpDirection.magnitude > jumpDeathZone && canJump)
        {
            rb.velocity = Vector2.zero;

            if ((jumpDirection / 50 * jumpForce).magnitude < maxForce)
                rb.AddForce(jumpDirection / 50 * jumpForce, ForceMode2D.Impulse);
            else
                rb.AddForce(jumpDirection.normalized * maxForce, ForceMode2D.Impulse);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Carrot") return;

        if (standingOn == "")
            standingOn = other.gameObject.name;

        //jazda na mob2 - w if'ie sprawdza czy gorna krawedz "Armor" jest nizej niz dolna krawedz boxcollider2d'a gracza
        if (other.gameObject.name == "Armor" && (other.transform.position.y + (other.collider.bounds.size.y / 2) < GetComponent<BoxCollider2D>().bounds.center.y - (GetComponent<BoxCollider2D>().bounds.size.y / 2)))
        {
            if (other.transform.root.GetComponent<Rigidbody2D>())
                rb.velocity = new Vector2(other.transform.root.GetComponent<Rigidbody2D>().velocity.x, rb.velocity.y);

            rb.gravityScale = 0f;
            canJump = true;
        }

        //skok na sciane
        else if (other.gameObject.tag == "Wall")
        {
            rb.gravityScale = 0f;
            rb.velocity = Vector2.zero;
            canJump = true;
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        //zmiana kierunku na mob2 kiedy player velocity i mob2 velocity maja rozne wartosci x
        if (other.gameObject.name == "Armor" && rb.velocity.y == 0f && other.transform.root.GetComponent<Rigidbody2D>().velocity.x != rb.velocity.x)
        {
            if (other.transform.root.GetComponent<Rigidbody2D>())
            {
                rb.velocity = new Vector2(other.transform.root.GetComponent<Rigidbody2D>().velocity.x, rb.velocity.y);
                rb.gravityScale = 0f;
                canJump = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag == "Carrot") return;

        if (other.gameObject.name == standingOn)
            standingOn = "";

        rb.gravityScale = 1f;
        canJump = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Carrot") return;
        if (collision.gameObject.name == "Armor") return;

        //kolizja przy staniu w bezruchu i odbicie gracza 
        if ((collision.tag == "Mob" || collision.tag == "HitPoint") && canTakeDMG && (rb.velocity == Vector2.zero || standingOn == "Armor"))
        {
            Vector2 knockBackDir = (transform.position - collision.gameObject.transform.position).normalized;

            //aby gracz nie odbijal sie W sciane
            if ((standingOn == "Floor" || standingOn == "Armor") && knockBackDir.y <= 0f)
            {
                knockBackDir.y = 0.2f;
                if (knockBackDir.x < 0f)
                    knockBackDir.x = -1f;
                else
                    knockBackDir.x = 1f;
            }
            else if (standingOn == "Left wall" && knockBackDir.x <= 0)
                knockBackDir.x = 0.3f;
            else if (standingOn == "Right wall" && knockBackDir.x >= 0)
                knockBackDir.x = -0.3f;
            else if (standingOn == "Roof" && knockBackDir.y >= 0)
            {
                knockBackDir.y = -0.2f;
                if (knockBackDir.x < 0f)
                    knockBackDir.x = -1f;
                else
                    knockBackDir.x = 1f;
            }

            rb.AddForce(knockBackDir * knockBackMultiplier, ForceMode2D.Impulse);

            if (playerHP > 0)
                TakeDMG();
        }
        //powinno byc w cast ale nie zawsze dziala wiec jest tutaj
        else if (collision.tag == "Mob" && canTakeDMG)
        {
            Vector2 knockBackDir = -rb.velocity.normalized;
            rb.velocity = Vector2.zero;
            rb.AddForce(knockBackDir * knockBackMultiplier, ForceMode2D.Impulse);

            if (playerHP > 0)
                TakeDMG();
        }
    }

    private void TakeDMG()
    {
        canTakeDMG = false;
        immunityTimer = 0f;
        playerHP--;
        GetComponent<SpriteRenderer>().color = new Color(0.85f, 0.4f, 0.4f);
    }
}
