using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Falling Rocks")]
    public GameObject indicator;
    public GameObject fallingRocks;
    public GameObject stillRocks;

    [Space(10)]

    [Header("Bullet Shoot")]
    public float bulletSpeed = 6f;
    public float bulletCount = 3f;
    public float betweenBulletCD = 0.5f;
    public GameObject bullet;

    [Space(10)]

    [Header("Bullet Series")]
    public float bulletSeriesSpeed = 6f;
    public float numberOfBullets = 10f;
    public float betweenBulletSeriesTime = 0.1f;

    [Space(10)]

    [Header("Pull In")]
    public float pullRange = 2f;
    public float pullForce = 4f;

    [Space(10)]

    [Header("Claster Bombs")]
    public float throwTime = 2f;
    public float clasterTime = 0.5f;
    public float mainBombScale = 2f;
    public float clasterDistance = 1f;
    public float clasterBombScale = -1f;
    public int clasterNumber = 8;
    public GameObject bomb;

    [Space(10)]

    [Header("Tail Attack")]
    public GameObject squareIndicator;
    public float attackTime = 2f;
    public float maxAngleAttack = 100;
    public float attackWidth = 1f;

    [HideInInspector]
    public bool isBulletBounce = false;



    private GameObject player;
    private float halfHeight;
    private float halfWidth;
    private GameObject[] rocks = new GameObject[5];
    private bool doPullIn = false;


    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        Camera camera = Camera.main;

        //obliczenia rozmiaru ekranu(kamery)
        halfHeight = camera.orthographicSize;
        halfWidth = camera.aspect * halfHeight;

        //szerokosc bossa na cala szerokosc ekranu
        transform.localScale = new Vector3(halfWidth * 2, transform.localScale.y, transform.localScale.z);
    }

    private void Update()
    {
        //atak PullIn
        if ((player.transform.position - transform.position).magnitude <= pullRange)
            doPullIn = true;
        else
            doPullIn = false;


        if (Input.GetKeyDown(KeyCode.R))
            StartCoroutine(AttackDropRocks());
        if (Input.GetKeyDown(KeyCode.E))
            StartCoroutine(BulletShoot());
        if (Input.GetKeyDown(KeyCode.W))
            StartCoroutine(BulletSeries());
        if (Input.GetKeyDown(KeyCode.Q))
            StartCoroutine(ClusterBombs());
        if (Input.GetKeyDown(KeyCode.A))
            StartCoroutine(TailAttack());
    }
    private void FixedUpdate()
    {
        if (doPullIn)
            PullIn();
    }

    //Atak spadajacych kamieni
    private IEnumerator AttackDropRocks()
    {
        Vector3[] spawnPos = new Vector3[5];

        for (int i = 0; i < 5; i++)
        {
            // pierwszy kamien losowany przy graczu reszta w losowym miejscu na ekranie, dla kazdego poza pierwszym losuje ponownie pozycje jesli sie powtarza wzgledem poprzednich
            if (i == 0)
            {
                spawnPos[0] = new Vector3(Random.Range(-halfWidth + 1, halfWidth - 1), Random.Range(-halfHeight + 1, halfHeight - 1));
                spawnPos[0] = player.transform.position + (spawnPos[0] - player.transform.position).normalized * 0.35f;
            }
            else
            {
                bool areAllDiff;
                do
                {
                    areAllDiff = true;
                    spawnPos[i] = new Vector3(Random.Range(-halfWidth + 1, halfWidth - 1), Random.Range(-halfHeight + 2, halfHeight - 2));

                    for (int j = 0; j < i; j++)
                    {
                        if (spawnPos[i].x > spawnPos[j].x - 1 && spawnPos[i].x < spawnPos[j].x + 1 && spawnPos[i].y > spawnPos[j].y - 1 && spawnPos[i].y < spawnPos[j].y + 1)
                        {
                            areAllDiff = false;
                            break;
                        }
                    }
                }
                while (!areAllDiff);
            }

            //ustawienie czasu zniszczenia indykatorow
            GameObject clone = Instantiate(indicator, spawnPos[i], Quaternion.identity);
            clone.GetComponent<DestroyMobIndicator>().destroyTime = 2;
        }

        yield return new WaitForSeconds(2);

        //podmiana gameobjectow- 2 stare, stojace kamienie
        var tmp = new GameObject[2];
        for (int i = 3; i < 5; ++i)
        {
            if (rocks[i])
                tmp[i-3] = rocks[i];
        }

        //stworzenie nowych spadajacych kamieni
        for (int i = 0; i < 5; i++)
            rocks[i] = Instantiate(fallingRocks, spawnPos[i], Quaternion.identity);

        yield return new WaitForSeconds(0.25f);

        //niszczenie starych stojacych kamieni jesli istnieja
        for (int i = 0; i < 2; i++)
            if (tmp[i])
                Destroy(tmp[i]);

        //niszczenie 3 pierwszych kamieni i stworzenie 2 stalych kamieni
        for (int i = 0; i < 5; i++)
        {
            Destroy(rocks[i]);

            if (i > 2)
                rocks[i] = Instantiate(stillRocks, spawnPos[i], Quaternion.identity);
        }

        yield return null;
    }

    //strzal seria pociskow, ktore odbijaja sie od powierzchni
    private IEnumerator BulletShoot()
    {
        isBulletBounce = true;

        Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z);

        //petla strzalu pociskow
        for (int i = 1; i <= bulletCount; i++)
        {
            Vector3 dir = (player.transform.position - transform.position).normalized;
            GameObject clone = Instantiate(bullet, spawnPos, Quaternion.identity);
            clone.GetComponent<Rigidbody2D>().velocity = dir * bulletSpeed;

            yield return new WaitForSeconds(betweenBulletCD);
        }

        yield return null;
    }

    //strzal seria po ekranie
    private IEnumerator BulletSeries()
    {
        isBulletBounce = false;

        Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z);

        //k¹t zmiany kierunku
        float angle = 2 / numberOfBullets;
        Vector3 dir = new Vector3(1, -1, 0).normalized;

        //losowanie poczatkowego kierunku
        if (Random.value >= 0.5f)
        {
            angle *= -1;
            dir.x *= -1;
        }

        //strzal pocisku
        while (dir.x >= -1 && dir.x <= 1)
        {
            GameObject clone = Instantiate(bullet, spawnPos, Quaternion.identity);
            clone.GetComponent<Rigidbody2D>().velocity = dir * bulletSeriesSpeed;

            dir.x -= angle;

            yield return new WaitForSeconds(betweenBulletSeriesTime);
        }

        yield return null;
    }

    //wciaganie gracza w bossa kiedy gracz jest za blisko
    private void PullIn()
    {
        {
            Rigidbody2D rbPlayer;
            if (player.GetComponent<Rigidbody2D>() && player.GetComponent<RabbitController>())
            {
                if (player.GetComponent<RabbitController>().canTakeDMG)
                {
                    rbPlayer = player.GetComponent<Rigidbody2D>();

                    //wciaga gracza w bossa, a nastepnie spowalnia ruch gracza po osi x
                    rbPlayer.AddForce((transform.position - player.transform.position).normalized * pullForce, ForceMode2D.Force);
                    rbPlayer.velocity = Vector2.Lerp(new Vector2(rbPlayer.velocity.x, rbPlayer.velocity.y), new Vector2(0, rbPlayer.velocity.y), 0.1f);
                }
            }
        }
    }

    //kasetowe bomby
    private IEnumerator ClusterBombs()
    {
        //pozycja glownej bomby = pozycja gracza w ograniczeniu do rozmiaru ekranu -2
        Vector3 pos = player.transform.position;
        if (pos.x < -halfWidth + 2)
            pos.x = -halfWidth + 2;
        if (pos.x > halfWidth - 2)
            pos.x = halfWidth - 2;
        if (pos.y < -halfHeight + 2)
            pos.y = -halfHeight + 2;
        if (pos.y > halfHeight - 2)
            pos.y = halfHeight - 2;

        //glowna bomba i jej zmienne(czas, rozmiar przy 0 = 2 poniewaz jest dodawany do domyslnego rozmiaru ktory wlasnie wynosi 2, czyli 2 + scale)
        var clone = Instantiate(bomb, pos, Quaternion.identity);
        if(clone.GetComponent<BombController>())
        {
            var bombClone = clone.GetComponent<BombController>();
            bombClone.bombScale = mainBombScale;
            bombClone.bombTimer = throwTime;
        }

        yield return new WaitForSeconds(throwTime);

        //liczba bomb kasetowych
        float range = 360 / clasterNumber;
        float angle = 0f;
        //bomby kasetowe i ich zmienne
        for (int i = 0; i < clasterNumber; i++)
        {
            Quaternion rotation = Quaternion.AngleAxis(angle, new Vector3(0, 0, -1));

            Vector3 dir = rotation * new Vector3(1, 0 , 0);

            var claster = Instantiate(bomb, pos + (dir * clasterDistance), Quaternion.identity);
            if(claster.GetComponent<BombController>())
            {
                var clasterBomb = claster.GetComponent<BombController>();
                clasterBomb.bombScale = clasterBombScale;
                //czas jest losowany w zakresie dla lepszego efektu
                clasterBomb.bombTimer = Random.Range(clasterTime - 0.25f, clasterTime + 0.25f);
            }
            angle += range;
        }

        yield return null;
    }

    private IEnumerator TailAttack()
    {
        //obliczenia kierunku, d³ugoœci ataku i pozycji pocz¹tkowej
        Vector3 dir = (player.transform.position - transform.position).normalized;
        float attackLength = (player.transform.position - transform.position).magnitude / 2;
        Vector3 pos = transform.position + dir * attackLength + dir * 0.25f;

        //spawn indykator
        var clone = Instantiate(squareIndicator, pos, Quaternion.identity);
        if(clone.GetComponent<SquareIndicatorController>())
            clone.GetComponent<SquareIndicatorController>().destroyTime = attackTime;

        //kierunek indykatora
        Quaternion targetRotation = Quaternion.FromToRotation(clone.transform.up, dir);
        clone.transform.rotation = targetRotation;
        //rozmiar indykatora
        clone.transform.localScale = new Vector3(clone.transform.localScale.x, attackLength * 2 + 0.5f, clone.transform.localScale.z);

        yield return new WaitForSeconds(attackTime);

        //spawn indykator, zmiana tagu i dodanie collidera
        var tail = Instantiate(squareIndicator, pos, Quaternion.identity);

        tail.tag = "Mob";

        tail.AddComponent<BoxCollider2D>();
        if (tail.GetComponent<BoxCollider2D>())
            tail.GetComponent<BoxCollider2D>().isTrigger = true;

        if (tail.GetComponent<SquareIndicatorController>())
            tail.GetComponent<SquareIndicatorController>().destroyTime = 0.1f;

        //kierunek ogona
        tail.transform.rotation = targetRotation;
        //rozmiar ogona
        tail.transform.localScale = new Vector3(tail.transform.localScale.x, attackLength * 2 + 0.5f, tail.transform.localScale.z);

        yield return null;
    }
}
