using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float baseSpeed;
    private float speed;
    private Vector3 movementChange;

    [Header("Dash")]
    public float dashSpeed;
    public bool dashing = false;

    [Header("Random")]
    public float invinciblityTime;
    private bool invincible;
    private float invinciblityTimeLeft;
    private Rigidbody2D rb2d;
    public SpriteRenderer currentSprite;
    private Animator playerAnim;

    [Header("Particles")]
    public GameObject deathParticles;
    public GameObject dustParticles;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<Animator>();
        Camera.main.GetComponent<CameraMovement>().target = gameObject.transform;
    }


    void Update()
    {
        GameManager.Instance.player = transform;
        movementChange = Vector2.zero;
        movementChange.x = Input.GetAxisRaw("Horizontal");  // GetAxisRaw goes straight to 1 instead of building up to it
        movementChange.y = Input.GetAxisRaw("Vertical");

        if (invincible == true)
        {
            if (invinciblityTimeLeft <= 0)
            {
                invincible = false;
            }
            else
            {
                invinciblityTimeLeft -= Time.deltaTime;
            }
        }

        if (movementChange != Vector3.zero)
        {
            if (dashing == false)
            {
                speed = baseSpeed;
                if ((movementChange.x == 1 || movementChange.x == -1) && (movementChange.y == 1 || movementChange.y == -1))  // When travelling diagonally
                    speed = speed / 1.3f;  // Slows you down
            }

            dustParticles.SetActive(true);
            if (movementChange.x > 0)
                currentSprite.flipX = false;
            else if (movementChange.x < 0)
                currentSprite.flipX = true;
            MoveCharacter();
        }
        else
        {
            dustParticles.SetActive(false);
            rb2d.velocity = Vector3.zero;
            playerAnim.SetBool("Walking", false);
        }
    }


    void MoveCharacter()
    {
        playerAnim.SetBool("Walking", true);
        rb2d.position = transform.position + movementChange * speed * Time.fixedDeltaTime;
    }


    public IEnumerator Dash(GameObject dashTrail)
    {
        speed = dashSpeed;
        dashing = true;
        if ((movementChange.x == 1 || movementChange.x == -1) && (movementChange.y == 1 || movementChange.y == -1))
            speed = speed / 1.5f;

        for (int i = 0; i <= 4;)
        {
            GameObject DashEffect = Instantiate(dashTrail, transform.position, dashTrail.transform.rotation);
            i++;
            yield return new WaitForSeconds(0.03f);
        }

        speed = baseSpeed;
        dashing = false;
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (dashing == false)
        {
            if ((collision.gameObject.CompareTag("Enemy") && collision.gameObject.GetComponent<Enemy>().moveSpeed != 0))
                Kill();

            else if (collision.gameObject.CompareTag("Magic") && collision.gameObject.GetComponent<Spell>().friendly == false)
                Kill();
        }
    }

    public void Kill()
    {
        if (invincible == false)
        {
            deathParticles.SetActive(true);
            deathParticles.transform.parent = transform.parent;
            GameManager.Instance.EndGame();
            Destroy(gameObject);
        }
    }
}
