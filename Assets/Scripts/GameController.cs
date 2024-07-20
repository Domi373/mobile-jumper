using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("Mobs")]
    public GameObject mob1;
    public GameObject mob2;
    public GameObject mob3;
    public GameObject mob4;
    public GameObject mob5;

    [Space(5)]
    public GameObject miniBoss1;
    public GameObject miniBoss2;
    public GameObject miniBoss3;
    public GameObject miniBoss4;
    public GameObject miniBoss5;

    [Space(5)]
    public GameObject boss;

    [Space(10)]

    [Header("Spawn Ratio in %")]
    public float mob1Ratio = 35;
    public float mob2Ratio = 15;
    public float mob3Ratio = 10;
    public float mob4Ratio = 5;
    public float mob5Ratio = 5;

    [Space(10)]

    [Header("Spawners")]
    public GameObject mobLeftSpawner;
    public GameObject mobRightSpawner;
    public GameObject mobCenterPointSpawner;
    public GameObject mobUpSpawner;

    [Space(10)]

    [Header("Screen edges")]
    public GameObject leftWall;
    public GameObject rightWall;
    public GameObject Roof;
    public GameObject Floor;

    [Space(10)]

    [Header("Canvas")]
    public TextMeshProUGUI textPlayerHP;
    public TextMeshProUGUI textScore;

    [Space(10)]

    [Header("Rest")]
    public GameObject indicator;
    public float spawnMiniBossScore;

    [HideInInspector]
    public float gameScore = 0f;

    private float timer = 0f;
    private GameObject player;
    private RabbitController playerController;
    private float value;
    private float halfHeight;
    private float halfWidth;
    private float startSpawnMiniBossScore;

    private void Start()
    {



        //do wywalenia- debuger bossa
        Instantiate(boss, new Vector3(0, 5, 0), Quaternion.identity);
        //-------------------------------




        startSpawnMiniBossScore = spawnMiniBossScore;

        if (player = GameObject.FindWithTag("Player"))
            playerController = player.GetComponent<RabbitController>();

        Camera camera = Camera.main;

        //obliczenia rozmiaru ekranu(kamery)
        halfHeight = camera.orthographicSize;
        halfWidth = camera.aspect * halfHeight;
        
        // pozycja krawedzi gry
        leftWall.transform.position = new Vector3(-halfWidth - leftWall.GetComponent<BoxCollider2D>().bounds.size.x / 2, 0, 0);
        rightWall.transform.position = leftWall.transform.position * -1;
        Roof.transform.position = new Vector3(0, halfHeight + Roof.GetComponent<BoxCollider2D>().bounds.size.y / 2, 0);
        Floor.transform.position = Roof.transform.position * -1;
        // skala krawedzi gry
        leftWall.transform.localScale = new Vector3(1, halfHeight * 2, 0);
        rightWall.transform.localScale = leftWall.transform.localScale;
        Roof.transform.localScale = new Vector3(halfWidth * 2, 1, 0);
        Floor.transform.localScale = Roof.transform.localScale;

        //pozycja spawnerow
        mobLeftSpawner.transform.position = new Vector3(-halfWidth - 2, 0, 0);
        mobRightSpawner.transform.position = new Vector3(halfWidth + 2, 0, 0);
        mobUpSpawner.transform.position = new Vector3(0, halfHeight + 1, 0);
        //skala spawnerow
        mobLeftSpawner.transform.localScale = new Vector3(1, halfHeight * 2 - 2, 0);
        mobRightSpawner.transform.localScale = mobLeftSpawner.transform.localScale;
        mobUpSpawner.transform.localScale = new Vector3(halfWidth * 2 - 2, 1, 0);

    }

    void Update()
    {
        timer += Time.deltaTime;


        if (gameScore / 100000 > 0.5f)
            value = 0.5f;
        else
            value = gameScore / 100000;

        // domyslnie co sekunde sie odbywa ten if | wraz ze scorem gry moby spawnuja sie szybciej | aktualnie value = <0.5;1> gdzie 0.5 osiaga w 100000 score 
        if (timer >= 1f - value)
        {

            //mob1 spawner
            if (Random.value * 100 <= mob1Ratio)
            {
                Mob1Spawner();
            }

            //mob2 spawner
            if (Random.value * 100 <= mob2Ratio)
            {
                Mob2Spawner();
            }

            //mob3 spawner
            if (Random.value * 100 <= mob3Ratio)
            {
                Mob3Spawner();
            }

            //mob4 spawner
            if (Random.value * 100 <= mob4Ratio)
            {
                Mob4Spawner();
            }

            //mob5 spawner
            if (Random.value * 100 <= mob5Ratio)
            {
                Mob5Spawner();
            }

            timer = 0f;
        }

        //spawn minibossow
        if (gameScore >= spawnMiniBossScore)
        {
            var x = Random.value * 100;

            if (x <= 20)
                MBoss1Spawner();
            else if (x <= 40)
                MBoss2Spawner();
            else if (x <= 60)
                MBoss3Spawner();
            else if (x <= 80)
                MBoss4Spawner();
            else
                MBoss5Spawner();

            spawnMiniBossScore += startSpawnMiniBossScore;
        }

        //update text hp player i score
        textPlayerHP.text = "HP: " + playerController.playerHP;
        textScore.text = "Score: " + gameScore;
    }

    private void Mob1Spawner()
    {
        //losowanie lewy-prawy spawner
        GameObject mobSpawner;
        if (Random.value > 0.5f)
            mobSpawner = mobLeftSpawner;
        else
            mobSpawner = mobRightSpawner;

        Vector3 spawnerPos = mobSpawner.transform.position;
        Vector2 correctSpawnerPos = new Vector2(spawnerPos.x, spawnerPos.y);

        //min-max pozycje Y spawnera
        Vector2 minSpawnPos = new Vector2(correctSpawnerPos.x, correctSpawnerPos.y - (mobSpawner.transform.localScale.y / 2));
        Vector2 maxSpawnPos = new Vector2(correctSpawnerPos.x, correctSpawnerPos.y + (mobSpawner.transform.localScale.y / 2));

        Vector2 mobSpawnPos = new Vector2(spawnerPos.x, Random.Range(minSpawnPos.y, maxSpawnPos.y));

        GameObject clone = Instantiate(mob1, mobSpawnPos, Quaternion.identity);

        //kierunek poczatkowego ruchu
        if (mobSpawner == mobLeftSpawner)
            clone.GetComponent<Mob1Controller>().moveDirection = Vector2.right;
        else
            clone.GetComponent<Mob1Controller>().moveDirection = Vector2.left;

        //inkrementacja predkosci z czasem gry | score 50000 = moveSpeed x2
        clone.GetComponent<Mob1Controller>().moveSpeed += gameScore / 50000;
        if (clone.GetComponent<Mob1Controller>().moveSpeed > 2)
            clone.GetComponent<Mob1Controller>().moveSpeed = 2;

    }

    private void Mob2Spawner()
    {
        //losowanie lewy-prawy spawner
        GameObject mobSpawner;
        if (Random.value > 0.5f)
            mobSpawner = mobLeftSpawner;
        else
            mobSpawner = mobRightSpawner;

        Vector3 spawnerPos = mobSpawner.transform.position;
        Vector2 correctSpawnerPos = new Vector2(spawnerPos.x, spawnerPos.y);

        //min-max pozycje Y spawnera
        Vector2 minSpawnPos = new Vector2(correctSpawnerPos.x, correctSpawnerPos.y - (mobSpawner.transform.localScale.y / 2 * 0.75f));
        Vector2 maxSpawnPos = new Vector2(correctSpawnerPos.x, correctSpawnerPos.y + (mobSpawner.transform.localScale.y / 2 * 0.75f));

        Vector2 mobSpawnPos = new Vector2(spawnerPos.x, Random.Range(minSpawnPos.y, maxSpawnPos.y));

        GameObject clone = Instantiate(mob2, mobSpawnPos, Quaternion.identity);

        //kierunek poczatkowego ruchu
        if (mobSpawner == mobLeftSpawner)
            clone.GetComponent<Mob2Controller>().moveDirection = Vector2.right;
        else
            clone.GetComponent<Mob2Controller>().moveDirection = Vector2.left;

        //ilosc hp z uplywem gry/wzrostem score przy 25000 i 50000 +1hp
        if (gameScore >= 50000)
            clone.GetComponent<Mob2Controller>().life += 2;
        else if (gameScore >= 25000)
            clone.GetComponent<Mob2Controller>().life += 1;
    }

    private void Mob3Spawner()
    {
        //losowy kierunek od srodka mapy
        Vector2 spawnDir = new Vector2(Random.value, Random.value).normalized;

        //odlegosc od srodka mapy
        float distanceFromCenter = Mathf.Pow(Mathf.Pow(halfHeight, 2) + Mathf.Pow(halfWidth, 2), 0.5f);

        //losowanie czy odwrocic wartosci x,y bo inaczej bylyby zawsze dodatnie
        if (Random.value > 0.5f)
            spawnDir.x *= -1;
        if (Random.value > 0.5f)
            spawnDir.y *= -1;

        Vector2 spawnPos = new Vector2(mobCenterPointSpawner.transform.position.x, mobCenterPointSpawner.transform.position.y) + (spawnDir * (distanceFromCenter + 1f));

        GameObject clone = Instantiate(mob3, spawnPos, Quaternion.identity);

        //inkrementacja predkosci z czasem gry | score 50000 = moveSpeed x2
        clone.GetComponent<Mob3Controller>().moveSpeed += gameScore / 50000;
        if (clone.GetComponent<Mob3Controller>().moveSpeed > 3)
            clone.GetComponent<Mob3Controller>().moveSpeed = 3;
    }

    private void Mob4Spawner()
    {
        float spawnMaxLeftPos = mobUpSpawner.transform.position.x - mobUpSpawner.transform.localScale.x / 2;
        float spawnMaxRightPos = mobUpSpawner.transform.position.x + mobUpSpawner.transform.localScale.x / 2;

        Vector2 mobSpawnPos = new Vector2(Random.Range(spawnMaxLeftPos, spawnMaxRightPos), mobUpSpawner.transform.position.y);

        GameObject clone = Instantiate(mob4, mobSpawnPos, Quaternion.identity);

        //cd i speed pocisku wzrasta | przy 50000 cd 1.5s, speed 9, movespeed 2x
        clone.GetComponent<Mob4Controller>().shootCD -= gameScore / 33333;
        if (clone.GetComponent<Mob4Controller>().shootCD < 1.5f)
            clone.GetComponent<Mob4Controller>().shootCD = 1.5f;
        clone.GetComponent<Mob4Controller>().shootSpeed += gameScore / 16666;
        if (clone.GetComponent<Mob4Controller>().shootSpeed > 9)
            clone.GetComponent<Mob4Controller>().shootSpeed = 9;
        clone.GetComponent<Mob4Controller>().moveSpeed += gameScore / 50000;
        if (clone.GetComponent<Mob4Controller>().moveSpeed > 2)
            clone.GetComponent<Mob4Controller>().moveSpeed = 2;

    }

    private void Mob5Spawner()
    {
        float spawnHorizontalPos = Random.Range(-halfWidth + 2, halfWidth - 2);
        float spawnVerticalPos = Random.Range(-halfHeight + 2, halfHeight - 2);

        Vector2 spawnPos = new Vector2(spawnHorizontalPos, spawnVerticalPos);

        //Najpierw spawn indykatora potem w korutynie spawn mob5
        Instantiate(indicator, spawnPos, Quaternion.identity);

        StartCoroutine(delayMob5Spawner(spawnPos));
    }

    IEnumerator delayMob5Spawner(Vector2 spawnPos)
    {
        yield return new WaitForSeconds(1.5f);

        GameObject clone = Instantiate(mob5, spawnPos, Quaternion.identity);

        //1 i nastepne bomby cd /2, promien bomby 2x wiekszy
        clone.GetComponent<Mob5Controller>().firstBombPutTimer -= gameScore / 50000;
        if (clone.GetComponent<Mob5Controller>().firstBombPutTimer < 1)
            clone.GetComponent<Mob5Controller>().firstBombPutTimer = 1;
        clone.GetComponent<Mob5Controller>().bombPutTimer -= gameScore / 25000;
        if (clone.GetComponent<Mob5Controller>().bombPutTimer < 2)
            clone.GetComponent<Mob5Controller>().bombPutTimer = 2;
        clone.GetComponent<Mob5Controller>().bombScale += gameScore / 25000;
        if (clone.GetComponent<Mob5Controller>().bombScale > 2)
            clone.GetComponent<Mob5Controller>().bombScale = 2;
    }

    private void MBoss1Spawner()
    {
        //losowanie lewy-prawy spawner
        GameObject mobSpawner;
        if (Random.value > 0.5f)
            mobSpawner = mobLeftSpawner;
        else
            mobSpawner = mobRightSpawner;

        Vector3 spawnerPos = mobSpawner.transform.position;
        Vector2 correctSpawnerPos = new Vector2(spawnerPos.x, spawnerPos.y);

        //min-max pozycje Y spawnera
        Vector2 minSpawnPos = new Vector2(correctSpawnerPos.x, correctSpawnerPos.y - (mobSpawner.transform.localScale.y / 2));
        Vector2 maxSpawnPos = new Vector2(correctSpawnerPos.x, correctSpawnerPos.y + (mobSpawner.transform.localScale.y / 2));

        Vector2 mobSpawnPos = new Vector2(spawnerPos.x, Random.Range(minSpawnPos.y, maxSpawnPos.y));

        GameObject clone = Instantiate(miniBoss1, mobSpawnPos, Quaternion.identity);

        //kierunek poczatkowego ruchu
        if (mobSpawner == mobLeftSpawner)
            clone.GetComponent<MBoss1Controller>().moveDirection = Vector2.right;
        else
            clone.GetComponent<MBoss1Controller>().moveDirection = Vector2.left;
    }

    private void MBoss2Spawner()
    {
        //losowanie lewy-prawy spawner
        GameObject mobSpawner;
        if (Random.value > 0.5f)
            mobSpawner = mobLeftSpawner;
        else
            mobSpawner = mobRightSpawner;

        Vector3 spawnerPos = mobSpawner.transform.position;
        Vector2 correctSpawnerPos = new Vector2(spawnerPos.x, spawnerPos.y);

        //min-max pozycje Y spawnera
        Vector2 minSpawnPos = new Vector2(correctSpawnerPos.x, correctSpawnerPos.y - (mobSpawner.transform.localScale.y / 2 * 0.75f));
        Vector2 maxSpawnPos = new Vector2(correctSpawnerPos.x, correctSpawnerPos.y + (mobSpawner.transform.localScale.y / 2 * 0.75f));

        Vector2 mobSpawnPos = new Vector2(spawnerPos.x, Random.Range(minSpawnPos.y, maxSpawnPos.y));

        GameObject clone = Instantiate(miniBoss2, mobSpawnPos, Quaternion.identity);

        //kierunek poczatkowego ruchu
        if (mobSpawner == mobLeftSpawner)
            clone.GetComponent<MBoss2Controller>().moveDirection = Vector2.right;
        else
            clone.GetComponent<MBoss2Controller>().moveDirection = Vector2.left;
    }

    private void MBoss3Spawner()
    {
        //losowy kierunek od srodka mapy
        Vector2 spawnDir = new Vector2(Random.value, Random.value).normalized;

        //odlegosc od srodka mapy
        float distanceFromCenter = Mathf.Pow(Mathf.Pow(halfHeight, 2) + Mathf.Pow(halfWidth, 2), 0.5f);

        //losowanie czy odwrocic wartosci x,y bo inaczej bylyby zawsze dodatnie
        if (Random.value > 0.5f)
            spawnDir.x *= -1;
        if (Random.value > 0.5f)
            spawnDir.y *= -1;

        Vector2 spawnPos = new Vector2(mobCenterPointSpawner.transform.position.x, mobCenterPointSpawner.transform.position.y) + (spawnDir * (distanceFromCenter + 3f));

        GameObject clone = Instantiate(miniBoss3, spawnPos, Quaternion.identity);

        //inkrementacja predkosci z czasem gry | score 50000 = moveSpeed x2
        clone.GetComponent<MBoss3Controller>().moveSpeed += gameScore / 50000;
        if (clone.GetComponent<MBoss3Controller>().moveSpeed > 2)
            clone.GetComponent<MBoss3Controller>().moveSpeed = 2;
    }

    private void MBoss4Spawner()
    {
        float spawnMaxLeftPos = mobUpSpawner.transform.position.x - mobUpSpawner.transform.localScale.x / 2;
        float spawnMaxRightPos = mobUpSpawner.transform.position.x + mobUpSpawner.transform.localScale.x / 2;

        Vector2 mobSpawnPos = new Vector2(Random.Range(spawnMaxLeftPos, spawnMaxRightPos), mobUpSpawner.transform.position.y);

        GameObject clone = Instantiate(miniBoss4, mobSpawnPos, Quaternion.identity);

        //cd i speed pocisku wzrasta | przy 50000 cd 1.5s, speed 9, movespeed 2x
        clone.GetComponent<MBoss4Controller>().shootCD -= gameScore / 33333;
        if (clone.GetComponent<MBoss4Controller>().shootCD < 1.5f)
            clone.GetComponent<MBoss4Controller>().shootCD = 1.5f;
        clone.GetComponent<MBoss4Controller>().shootSpeed += gameScore / 16666;
        if (clone.GetComponent<MBoss4Controller>().shootSpeed > 9)
            clone.GetComponent<MBoss4Controller>().shootSpeed = 9;
        clone.GetComponent<MBoss4Controller>().moveSpeed += gameScore / 50000;
        if (clone.GetComponent<MBoss4Controller>().moveSpeed > 2)
            clone.GetComponent<MBoss4Controller>().moveSpeed = 2;
    }

    private void MBoss5Spawner()
    {
        float spawnHorizontalPos = Random.Range(-halfWidth + 2, halfWidth - 2);
        float spawnVerticalPos = Random.Range(-halfHeight + 2, halfHeight - 2);

        Vector2 spawnPos = new Vector2(spawnHorizontalPos, spawnVerticalPos);

        GameObject clone = Instantiate(miniBoss5, spawnPos, Quaternion.identity);

        //cd momby z 2 na 1, speed z 1 na 2, rotation z 2.5 na 3.5 przy 50000 score
        clone.GetComponent<MBoss5Controller>().moveSpeed += gameScore / 50000;
        if (clone.GetComponent<MBoss5Controller>().moveSpeed > 2)
            clone.GetComponent<MBoss5Controller>().moveSpeed = 2;
        clone.GetComponent<MBoss5Controller>().rotationSpeed += gameScore / 50000;
        if (clone.GetComponent<MBoss5Controller>().rotationSpeed > 3.5f)
           clone.GetComponent<MBoss5Controller>().rotationSpeed = 3.5f;
        clone.GetComponent<MBoss5Controller>().bombCD -= gameScore / 50000;
        if (clone.GetComponent<MBoss5Controller>().bombCD < 1)
            clone.GetComponent<MBoss5Controller>().bombCD = 1;
    }
}
