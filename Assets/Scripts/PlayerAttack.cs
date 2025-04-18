using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject player;
    private Vector2 lookDirection;
    private float lookAngle;


    public enum Attack { Fireball, Light, Poison, Dark, Ice, None}
    public Attack currentAttack = Attack.None;
    public float attackCooldown;
    public float rerollCooldown;
    public GameObject firePoint;

    [Header("Fireball")]
    public float fireballCooldown;
    public float fireballAccuracy;
    public GameObject fireballGameobject;

    [Header("Light")]
    public float lightCooldown;
    public float lightAccuracy;
    public GameObject lightGameobject;

    [Header("Poison")]
    public float poisonCooldown;
    public float poisonAccuracy;
    public GameObject poisonGameobject;

    [Header("Dark")]
    public float darkCooldown;
    public GameObject darkGameobject;

    [Header("Ice")]
    public float iceCooldown;
    public GameObject iceGameobject;
    public GameObject iceShardGameobject;

    [Header("Reroll")]
    public float rerollCooldownTime;
    public SpriteRenderer dice;
    public GameObject diceToThrow;
    private GameObject thrownDice;
    public Sprite[] diceImages;
    private int lastType = 5;
    private bool readyForPickup;



    void Update()
    {
        lookDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        lookAngle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, lookAngle - 90f);


        if (attackCooldown <= 0)
        {
            if (Input.GetButton("Attack"))
            {
                if (currentAttack == Attack.Fireball)
                    Fireball();
                else if (currentAttack == Attack.Light)
                    Light();
                else if (currentAttack == Attack.Poison)
                    Poison();
                else if (currentAttack == Attack.Dark)
                    Dark();
                else if (currentAttack == Attack.Ice)
                    Ice();
            }
        }
        else
            attackCooldown -= Time.deltaTime * 100;





        if (rerollCooldown <= 0)
        {
            if (Input.GetButtonDown("Reroll"))
            {
                StartCoroutine(AttackReroll());
            }
        }
        else if (!readyForPickup && dice.sprite != null)
            rerollCooldown -= Time.deltaTime * 100;
    }



    void Fireball()
    {
        attackCooldown = fireballCooldown;
        Quaternion shotAccuracy = Quaternion.AngleAxis(Random.Range(-fireballAccuracy, fireballAccuracy), Vector3.forward);
        GameObject firedMagic = Instantiate(fireballGameobject, firePoint.transform.position, this.transform.rotation * shotAccuracy);
        Spell spell = firedMagic.gameObject.GetComponent<Spell>();
        firedMagic.GetComponent<Rigidbody2D>().velocity = firedMagic.transform.up * spell.speed;
        Destroy(firedMagic, spell.destroyTime);
    }

    void Light()
    {
        attackCooldown = lightCooldown;
        Quaternion shotAccuracy = Quaternion.AngleAxis(Random.Range(-lightAccuracy, lightAccuracy), Vector3.forward);
        GameObject firedMagic = Instantiate(lightGameobject, firePoint.transform.position, this.transform.rotation * shotAccuracy);
        Spell spell = firedMagic.gameObject.GetComponent<Spell>();
        firedMagic.GetComponent<Rigidbody2D>().velocity = firedMagic.transform.up * spell.speed;
        Destroy(firedMagic, spell.destroyTime);
    }

    void Poison()
    {
        attackCooldown = poisonCooldown;
        Quaternion shotAccuracy = Quaternion.AngleAxis(Random.Range(-poisonAccuracy, poisonAccuracy), Vector3.forward);
        GameObject firedMagic = Instantiate(poisonGameobject, firePoint.transform.position, this.transform.rotation * shotAccuracy);
        Spell spell = firedMagic.gameObject.GetComponent<Spell>();
        firedMagic.GetComponent<Rigidbody2D>().velocity = firedMagic.transform.up * spell.speed;
        Destroy(firedMagic, spell.destroyTime);
    }

    void Dark()
    {
        attackCooldown = darkCooldown;
        StartCoroutine(player.GetComponent<PlayerMovement>().Dash(darkGameobject));
    }

    void Ice()
    {
        attackCooldown = iceCooldown;
        Quaternion shotAccuracy = Quaternion.AngleAxis(Random.Range(0, 300), Vector3.forward);
        GameObject firedMagic = Instantiate(iceGameobject, transform.position, this.transform.rotation * shotAccuracy);

        for (int i = 0; i <= Random.Range(4, 7);)
        {
            shotAccuracy = Quaternion.AngleAxis(Random.Range(0, 300), Vector3.forward);
            firedMagic = Instantiate(iceShardGameobject, transform.position, this.transform.rotation * shotAccuracy);
            Spell spell = firedMagic.gameObject.GetComponent<Spell>();
            firedMagic.GetComponent<Rigidbody2D>().velocity = firedMagic.transform.up * spell.speed;
            Destroy(firedMagic, spell.destroyTime);
            i++;
        }
    }

    private IEnumerator AttackReroll()
    {
        rerollCooldown = rerollCooldownTime;
        currentAttack = Attack.None;
        dice.sprite = null;
        readyForPickup = false;
        thrownDice = Instantiate(diceToThrow, firePoint.transform.position, this.transform.rotation);
        GameManager.Instance.dice = thrownDice.transform;
        thrownDice.GetComponent<Rigidbody2D>().velocity = thrownDice.transform.up * 3;
        thrownDice.GetComponent<Rigidbody2D>().AddTorque(1.5f, ForceMode2D.Impulse);

        int y = lastType;
        for (int i = 0; i <= 15;)
        {
            int x = Random.Range(0, diceImages.Length);
            if (x != y && x != 5)
            {
                thrownDice.GetComponent<SpriteRenderer>().sprite = diceImages[x];
                y = x;
                i++;
                yield return new WaitForSeconds(0.15f);
            }
        }

        thrownDice.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        thrownDice.GetComponent<Rigidbody2D>().freezeRotation = true;
        readyForPickup = true;

        for (int i = 0; i != 1;)
        {
            int x = Random.Range(0, diceImages.Length);
            if (x != lastType)
            {
                thrownDice.GetComponent<SpriteRenderer>().sprite = diceImages[x];
                lastType = x;
                break;
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Dice") && readyForPickup)
        {
            readyForPickup = false;
            dice.sprite = diceImages[lastType];
            if (lastType == 0)
                currentAttack = Attack.Fireball;
            else if (lastType == 1)
                currentAttack = Attack.Ice;
            else if (lastType == 2)
                currentAttack = Attack.Poison;
            else if (lastType == 3)
                currentAttack = Attack.Light;
            else if (lastType == 4)
                currentAttack = Attack.Dark;
            else if (lastType == 5)
                currentAttack = Attack.None;

            rerollCooldown = rerollCooldownTime;
            thrownDice.transform.Find("Destroy Particles").gameObject.SetActive(true);
            thrownDice.transform.Find("Destroy Particles").transform.parent = transform.parent;
            Destroy(thrownDice);
        }
    }
}
