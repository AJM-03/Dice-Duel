using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public List<GameObject> enemies;

    public int highscore;
    public int score;
    public int difficulty = 0;

    
    public GameObject[] easyEnemyList;
    public GameObject[] midEnemyList;
    public GameObject[] hardEnemyList;
    public float timeBetweenSpawns;
    private float timeTillSpawn = 10;
    public int maxEnemies;
    public Transform dice, player;
    public Animator hideUI;
    public TMP_Text scoreText;
    public GameObject tabletObject, playerObject;

    void Start()
    {
        if (Instance != null && Instance != this)
            Destroy(this.gameObject);
        else
            Instance = this;
    }

    void Update()
    {
        if (scoreText == null)
            scoreText = hideUI.transform.parent.Find("Score").GetComponentInChildren<TMP_Text>();

        if (score == 0)
            scoreText = null;
        else
            scoreText.text = (score / 10).ToString();

        if (score >= 1 && difficulty == 0)
        {
            hideUI.transform.parent.gameObject.SetActive(true);
            hideUI.transform.parent.Find("Score").gameObject.SetActive(true);
            hideUI.SetTrigger("Dissapear");
            difficulty = 1;
            timeTillSpawn = 5;
        }
        else if (score >= 250 && difficulty == 1)
            difficulty = 2;
        else if (score >= 1000 && difficulty == 2)
            difficulty = 3;

        if (timeTillSpawn <= 0 && enemies.Count <= maxEnemies)
            SpawnEnemy();

        else if (enemies.Count >= maxEnemies || difficulty == 0)
            timeTillSpawn = timeBetweenSpawns;

        else
            timeTillSpawn -= Time.deltaTime * 10;
    }

    void SpawnEnemy()
    {
        timeTillSpawn = timeBetweenSpawns;
        GameObject enemyToSpawn = null;

        if (difficulty == 1)
            enemyToSpawn = midEnemyList[Random.Range(0, easyEnemyList.Length)];
        else if (difficulty == 2)
            enemyToSpawn = midEnemyList[Random.Range(0, midEnemyList.Length)];
        else if (difficulty == 3)
            enemyToSpawn = midEnemyList[Random.Range(0, hardEnemyList.Length)];

        float randX = Random.Range(-14, 14);
        float randY = Random.Range(-8, 8);
        GameObject enemy = Instantiate(enemyToSpawn, new Vector2(randX, randY), Quaternion.identity);
        if (player != null && Vector2.Distance(enemy.transform.position, player.position) < 10)
        {
            Destroy(enemy);
            SpawnEnemy();
        }
    }

    public void EndGame()
    {
        StartCoroutine(End());
    }

    private IEnumerator End()
    {
        Time.timeScale = 0.2f;

        yield return new WaitForSeconds(1);

        Time.timeScale = 1;

        Instantiate(playerObject, new Vector2(0, -3), Quaternion.identity);
        Instantiate(tabletObject, new Vector2(0, 2), Quaternion.identity);

        if (score > highscore)
            highscore = score;

        score = 0;
        difficulty = 0;
        hideUI.transform.parent.gameObject.SetActive(false);
        hideUI.gameObject.SetActive(false);
        scoreText = hideUI.transform.parent.Find("Score").GetComponentInChildren<TMP_Text>();
        scoreText = null;

        for (int i = 0; i < enemies.Count; i++)  // Move away from other enemies
        {
            Destroy(enemies[i]);
        }
    }
}
