using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy_Coffin : MonoBehaviour
{
    private Animator animator;
    private Transform playerTransform;
    private float attackTimer;
    private Rigidbody2D rb;
    private bool canAttack = true;
    public float chaseTriggerDistance = 3f;

    public float moveSpeed = 2f;
    public float attackRange = 1f;
    public float attackCooldown = 2f; // Adjust this value as needed

    private bool isAttacking = false;
    public SpriteRenderer sprite;

    public int scoreValue;

    public int hitpoints;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        if (attackTimer <= 0 && isAttacking)
        {
            isAttacking = false; // Reset the attacking flag
            animator.SetBool("IsAttacking", false); // Ensure the animator is updated
        }
    }
    void FixedUpdate()
    {
        if (playerTransform == null)
        {
            return;
        }

        // Calculate the distance to the player
        float distanceToPlayer = Vector2.Distance(playerTransform.position, rb.position);

        // If the player is within attack range and the attack timer has elapsed, attack the player
        if (distanceToPlayer < attackRange && attackTimer <= 0)
        {
            Attack();
        }
        else if (distanceToPlayer >= attackRange)
        {
            // If the player is outside the attack range, reset the canAttack flag so the enemy can attack again when the player comes close next time
            canAttack = true;

            // Optionally, you can make the enemy chase the player if they are within a certain range
            if (distanceToPlayer < chaseTriggerDistance)
            {
                ChasePlayer();
            }
            else
            {
                rb.velocity = Vector2.zero;
                animator.SetBool("IsMoving", false);
            }
        }

    }
    void ChasePlayer()
    {
        // Move towards the player
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        rb.velocity = direction * moveSpeed;
        animator.SetBool("IsMoving", true);

        // Set the animation parameters
        animator.SetFloat("MoveX", Mathf.Abs(direction.x));
        animator.SetFloat("MoveY", direction.y);

        // Flip the sprite based on the direction
        GetComponent<SpriteRenderer>().flipX = direction.x < 0;
    }


    void Attack()
    {
        if (canAttack)
        {
            // Perform the attack
            Debug.Log("Coffin attacks the player!");

            // Reset the attack timer to the cooldown period
            attackTimer = attackCooldown;

            // Trigger the attack animation
            animator.SetTrigger("Attack");

            // Set the flag so the enemy does not attack again until the cooldown is finished
            canAttack = false;
        }
    }

    public void ResetAttack()
    {
        canAttack = true;
    }

    //TODO: This should be moved to the BulletController
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player-Bullet"))
        {
            int dmg = collision.gameObject.GetComponent<BulletController>().damage;
            TakeDamage(dmg);
            Destroy(collision.gameObject);
        }
    }

    void Die()
    {
        animator.SetTrigger("Death");
        Score.AddScore(scoreValue);
        this.enabled = false;
        //GetComponent<Collider2D>().enabled = false; 
        StartCoroutine(FadeAlpha());
        GetComponent<Rigidbody2D>().isKinematic = true;
    }

    public void TakeDamage(int damage)
    {
        hitpoints -= damage;
        if (hitpoints <= 0)
        {
            Die();
        }
        else
        {
            StartCoroutine(FlashRed());
        }
    }

    public IEnumerator FlashRed()
    {
        Debug.Log("Trying to change color");
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sprite.color = Color.white;
    }

    public IEnumerator FadeAlpha()
    {
        float duration = 0.7f;
        float startTime = Time.time;
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        while (Time.time < startTime + duration)
        {
            Color clr = renderer.color;
            Color newColor = new Color(clr.r, clr.g, clr.b, clr.a - (Time.deltaTime / duration));
            renderer.color = newColor;
            yield return null;
        }
    }

    public void DestroyEnemy()
    {
        Destroy(gameObject);
    }

}
