using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    public bool follow;
    public float moveSpeed;
    public float moveAwayDistance;
    public float chaseDistance;
    private float enemyDistance;

    [Header("Shooting")]
    public float shootingDistance;
    public float accuracy;
    private float timeBtwShots;
    public float startTimeBtwShots;
    public float timeBtwShotsMaxAdd;
    private float randTimeBtwShotsAdd;
    public int shotsAtOnce = 1;
    private int repeatShot;

    private Vector2 lookDirection;
    private float lookAngle;

    [Header("Setup")]
    public int health;
    private int healthRemaining;
    public PlayerAttack.Attack immunity;
    public GameObject firePoint;
    public GameObject projectile;
    private Transform player;
    public GameObject deathParticles;
    private float hurtFrames;
    public Color32 hurtColour, defaultColour, immuneColour;
    public SpriteRenderer spriteRend;
    private Animator enemyAnim;
    private float tickDamageCountdown;
    private Vector3 actualPos, previousPos;
    private Quaternion startRotation;


    void Start()
    {
        if (GameObject.FindGameObjectWithTag("Player"))
            player = GameObject.FindGameObjectWithTag("Player").transform;
        healthRemaining = health;
        enemyAnim = GetComponent<Animator>();
        startRotation = transform.rotation;
        timeBtwShots = startTimeBtwShots;
        enemyDistance = Random.Range(1.1f, 6.1f);
        if (moveSpeed != 0)
        {
            moveSpeed = moveSpeed + Random.Range(-1f, 1f);
            GameManager.Instance.enemies.Add(gameObject);
        }
        if (!follow)
        {
            chaseDistance = moveSpeed + Random.Range(-1.1f, 1.1f);
            moveAwayDistance = moveAwayDistance + Random.Range(-1.1f, 1.1f);
        }
    }

    void Update()
    {
        if (player != null)
        {
            if (spriteRend.color != defaultColour && hurtFrames <= 0)
                spriteRend.color = defaultColour;
            else if (hurtFrames > 0)
                hurtFrames -= Time.deltaTime * 10;

            for (int i = 0; i < GameManager.Instance.enemies.Count; i++)  // Move away from other enemies
            {
                if (GameManager.Instance.enemies[i] != null && Vector2.Distance(transform.position, GameManager.Instance.enemies[i].transform.position) < enemyDistance)
                    transform.position = Vector2.MoveTowards(transform.position, GameManager.Instance.enemies[i].transform.position, -moveSpeed * Time.deltaTime);
            }


            if (GameManager.Instance.dice != null && GameManager.Instance.dice.GetComponent<Rigidbody2D>().velocity == Vector2.zero && Vector2.Distance(transform.position, GameManager.Instance.dice.position) < enemyDistance)
                transform.position = Vector2.MoveTowards(transform.position, GameManager.Instance.dice.position, -moveSpeed * Time.deltaTime);


            if (Vector2.Distance(transform.position, player.position) > chaseDistance)
                transform.position = Vector2.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);

            lookDirection = player.transform.position - transform.position;
            lookAngle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;

            if ((lookAngle > -90 && lookAngle <= 0) || (lookAngle > 0 && lookAngle <= 90))
                spriteRend.flipX = false;
            else if ((lookAngle > 90 && lookAngle <= 180) || (lookAngle > -180 && lookAngle <= -90))
                spriteRend.flipX = true;


            if (!follow)
            {
                if (Vector2.Distance(transform.position, player.position) < moveAwayDistance)
                    transform.position = Vector2.MoveTowards(transform.position, player.position, -moveSpeed * Time.deltaTime);

                firePoint.transform.parent.rotation = Quaternion.Euler(0f, 0f, lookAngle - 90f);


                if (timeBtwShots <= 0)
                {
                    if (Vector2.Distance(transform.position, player.position) < shootingDistance)
                    {
                        Quaternion shotAccuracy = Quaternion.AngleAxis(Random.Range(-accuracy, accuracy), Vector3.forward);
                        GameObject firedMagic = Instantiate(projectile, firePoint.transform.position, firePoint.transform.parent.rotation * shotAccuracy);
                        Spell magic = firedMagic.gameObject.GetComponent<Spell>();

                        firedMagic.GetComponent<Rigidbody2D>().velocity = firePoint.transform.up * magic.speed;
                        magic.friendly = false;
                        Destroy(firedMagic, magic.destroyTime);

                        if (shotsAtOnce != 1 && repeatShot == 0)
                        {
                            randTimeBtwShotsAdd = Random.Range(0, timeBtwShotsMaxAdd);
                            timeBtwShots = startTimeBtwShots + randTimeBtwShotsAdd;
                            repeatShot = shotsAtOnce;
                        }
                        else if (shotsAtOnce == 1)
                        {
                            randTimeBtwShotsAdd = Random.Range(0, timeBtwShotsMaxAdd);
                            timeBtwShots = startTimeBtwShots + randTimeBtwShotsAdd;
                        }
                        else
                            repeatShot--;
                    }
                }
                else
                {
                    timeBtwShots -= Time.deltaTime * 100;
                }
            }

            transform.rotation = startRotation;


            actualPos = transform.position;
            if (Vector2.Distance(previousPos, actualPos) < 0.01)
                enemyAnim.SetBool("Walking", false);
            else
                enemyAnim.SetBool("Walking", true);

            previousPos = transform.position;
        }
        else
            if (GameObject.FindGameObjectWithTag("Player"))
            player = GameObject.FindGameObjectWithTag("Player").transform;
    }



    public void Hurt(int damage, PlayerAttack.Attack type)
    {
        if (type != immunity)
        {
            spriteRend.color = hurtColour;
            hurtFrames = 2;
            healthRemaining -= damage;

            if (healthRemaining <= 0)
            {
                GameManager.Instance.enemies.Remove(gameObject);
                GameManager.Instance.score += health;
                deathParticles.gameObject.SetActive(true);
                deathParticles.transform.parent = transform.parent;
                Destroy(gameObject);
            }
        }
        else
        {
            spriteRend.color = immuneColour;
            hurtFrames = 2;
        }
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Tick Damage"))
        {
            tickDamageCountdown -= Time.deltaTime * 10;

            if (tickDamageCountdown <= 0)
            {
                Hurt(collision.GetComponent<TickDamage>().damage, collision.GetComponent<TickDamage>().element);
                tickDamageCountdown = 10;
            }
        }
    }
}
