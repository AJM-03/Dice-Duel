using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public PlayerAttack.Attack element;
    public float speed;
    public float destroyTime;
    public int damage;
    public bool friendly = true;
    public bool trap = false;
    public bool spawn = false;
    public bool shield = false;
    public bool indestructible = false;
    public bool canBreak = true;
    public bool canBreakStrong = false;
    public float spinSpeed;


    private Transform player;
    private Rigidbody2D rb2d;
    private float timeTillFreeze;
    public GameObject enemyToSpawn;
    public GameObject destroyParticles;
    public GameObject spawnParticles;



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (friendly == true && !collision.gameObject.CompareTag("Player") && !collision.gameObject.CompareTag("Magic Ignore") && !collision.gameObject.CompareTag("Tick Damage"))
        {
            if (collision.gameObject.CompareTag("Enemy"))
            {
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();
                destroyParticles.SetActive(true);
                destroyParticles.transform.parent = transform.parent;
                enemy.Hurt(damage, element);
            }

            if (collision.gameObject.CompareTag("Magic") && collision.GetComponent<Spell>().friendly == true) { }

            else if (indestructible == false)
            {
                destroyParticles.SetActive(true);
                destroyParticles.transform.parent = transform.parent;
                Destroy(gameObject);
            }

            if (spawn == true)
            {
                Instantiate(enemyToSpawn, transform.position, enemyToSpawn.transform.rotation);
                spawnParticles.SetActive(true);
                spawnParticles.transform.parent = transform.parent;
                Destroy(gameObject);
            }
        }

        else if (friendly == false)
        {
            if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Magic Ignore") || collision.gameObject.CompareTag("Tick Damage") || collision.gameObject.CompareTag("Player")) { }

            else if (indestructible == false)
            {
                destroyParticles.SetActive(true);
                destroyParticles.transform.parent = transform.parent;
                Destroy(gameObject);
            }
        }
    }



    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.AddTorque(spinSpeed, ForceMode2D.Impulse);

        if (trap == true && !friendly)
            timeTillFreeze = Random.Range(0.25f, 2f);
        if (spawn == true)
            timeTillFreeze = Random.Range(0.25f, 1.4f);
    }



    private void Update()
    {
        if (trap == true)
        {
            if (timeTillFreeze <= 0)
            {
                rb2d.velocity = Vector3.zero;
                rb2d.constraints = RigidbodyConstraints2D.FreezeAll;
            }
            else
            {
                timeTillFreeze -= Time.deltaTime;
            }
        }

        if (spawn == true)
        {
            if (timeTillFreeze <= 0)
            {
                GameObject enemy = Instantiate(enemyToSpawn, transform.position, enemyToSpawn.transform.rotation);
                if (enemy.GetComponent<Animator>())
                    enemy.GetComponent<Animator>().SetBool("Spawned", true);
                spawnParticles.SetActive(true);
                spawnParticles.transform.parent = transform.parent;
                Destroy(gameObject);
            }
            else
            {
                timeTillFreeze -= Time.deltaTime;
            }
        }
    }
}
