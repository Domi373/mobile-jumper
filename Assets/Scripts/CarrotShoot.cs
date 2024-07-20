using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CarrotShoot : MonoBehaviour
{
    public float shootSpeed = 10f;
    public float shootCD = 1f;
    public GameObject carrot;

    private Vector2 clickPosition;
    private Vector2 currentClickPosition;
    private Rigidbody2D rb;
    private Camera cam;
    private RabbitController controller;
    private float shootTimer = 0f;


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        controller = GetComponent<RabbitController>();
    }

    private void Update()
    {
        if (shootTimer > 0f) shootTimer -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0))
        {
            clickPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        }

        if (Input.GetMouseButtonUp(0))
        {
            currentClickPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            if ((clickPosition - currentClickPosition).magnitude < controller.jumpDeathZone && shootTimer <= 0f)
            {
                Vector3 mouseToWorld = cam.ScreenToWorldPoint(new Vector3(currentClickPosition.x, currentClickPosition.y, 0f));
                Vector2 mouseConvertPos = new Vector2(mouseToWorld.x, mouseToWorld.y);
                Vector2 pos = new Vector2(rb.position.x + GetComponent<BoxCollider2D>().offset.x * 2, rb.position.y + GetComponent<BoxCollider2D>().offset.y * 2);
                GameObject clone = Instantiate(carrot, pos, Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.right, mouseConvertPos - rb.position) + 135));


                Rigidbody2D rbClone = clone.GetComponent<Rigidbody2D>();

                rbClone.velocity = (mouseConvertPos - rb.position).normalized * shootSpeed;

                shootTimer = shootCD;
            }
        }
    }
}
