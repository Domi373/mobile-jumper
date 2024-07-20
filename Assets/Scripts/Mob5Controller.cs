using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Mob5Controller : MonoBehaviour
{
    public float score = 200f;
    public float firstBombPutTimer = 2f;
    public float bombPutTimer = 4f;
    public GameObject bomb;

    [HideInInspector]
    public float bombScale;

    private GameObject player;
    private float time = 0f;
    private bool didPutFirstBomb = false;

    void Start()
    {
        if (GameObject.FindGameObjectWithTag("Player"))
            player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        time += Time.deltaTime;
        if ((time >= firstBombPutTimer && !didPutFirstBomb) || time >= bombPutTimer)
        {
            didPutFirstBomb = true;

            Vector2 putBombDir = player.transform.position - transform.position;
            Vector2 putBombLoc = new Vector2(transform.position.x, transform.position.y) + putBombDir.normalized * 0.5f;

            //promien bomby
            GameObject clone = Instantiate(bomb, putBombLoc, Quaternion.identity);
            clone.GetComponent<BombController>().bombScale = bombScale;

            time = 0f;
        }
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
