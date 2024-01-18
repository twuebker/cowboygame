using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Cactus_Behavior : MonoBehaviour, IDamageable
{
    public float attackRange = 1f;
    public float attackCooldown = 1f;
    public LayerMask attackMask;
    private AIPath aiPath;
    private Animator animator;
    private Transform player;
    private bool isAttacking = false;
    private bool coolingDown = false;
    private float lastAttackTime = 0f;
    private SpriteRenderer sprite;
    public int scoreValue = 10;
    public float _health = 200f;
    public GameObject bulletPrefab;

    public float Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;
            if (_health <= 0)
            {
                Die();
            }
            else
            {
                StartCoroutine(FlashRed());
            }
        }
    }

    public void OnHit(float damage)
    {
        Health -= damage;
    }

    public IEnumerator FlashRed()
    {
        sprite.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sprite.color = Color.white;
    }

    public IEnumerator FadeAlpha()
    {
        float duration = 0.7f;
        float startTime = Time.time;
        while (Time.time < startTime + duration)
        {
            Color clr = sprite.color;
            Color newColor = new Color(clr.r, clr.g, clr.b, clr.a - (Time.deltaTime / duration));
            sprite.color = newColor;
            yield return null;
        }
    }

    void Start()
    {
        aiPath = GetComponent<AIPath>();
        animator = GetComponent<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();

        // Declare the playerObject variable within the Start method
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            this.enabled = false;
            return;
        }
        AIDestinationSetter aiDestinationSetter = GetComponent<AIDestinationSetter>();
        aiDestinationSetter.target = player;
    }


    void Update()
    {
        // Check if the player has been destroyed and stop executing if it has
        if (player == null)
        {
            aiPath.canMove = false;
            return; // Exit the Update method early
        }

        if (!isAttacking)
        {
            aiPath.canMove = true;
            UpdateAnimation(aiPath.desiredVelocity);
            CheckForAttackOpportunity();
        }
    }

    void CheckForAttackOpportunity()
    {
        // Check if the player has been destroyed
        if (player == null) return;

        Vector2 directionToPlayer = player.position - transform.position;
        Vector2 moveDirection = aiPath.desiredVelocity.normalized;

        // Calculate the dot product to determine if the player is in front of the character
        float dotProduct = Vector2.Dot(moveDirection, directionToPlayer.normalized);

        // Check if player is within attack range, in front of character, and if cooldown has passed
        if (directionToPlayer.magnitude < attackRange && dotProduct > 0 && !coolingDown)
        {
            if (Time.time > lastAttackTime + attackCooldown)
            {
                Attack();
            }
        }
    }


    void UpdateAnimation(Vector2 velocity)
    {
        bool isMoving = aiPath.canMove && velocity.sqrMagnitude > 0.1f;
        animator.SetBool("canWalk", isMoving);

        if (isMoving)
        {
            Vector2 localDirection = transform.InverseTransformDirection(velocity.x, velocity.y, 0);
            animator.SetFloat("moveX", localDirection.x);
            animator.SetFloat("moveY", localDirection.y);
            GetComponentInChildren<SpriteRenderer>().flipX = localDirection.x < 0;
        }
    }

    void Attack()
    {
        lastAttackTime = Time.time;
        isAttacking = true;
        aiPath.canMove = false;
        animator.SetBool("canWalk", false);
        animator.SetTrigger("shoot");

        StartCoroutine(ShootBullet());
    }

public IEnumerator ShootBullet()
{
    yield return new WaitForSeconds(0.1f); 

    float idleX = animator.GetFloat("moveX");
    float idleY = animator.GetFloat("moveY");

    if (idleX == 0 && idleY == 0)
    {
        idleX = player.position.x - transform.position.x;
        idleY = player.position.y - transform.position.y;
    }

    float angle = Mathf.Atan2(idleY, idleX) * Mathf.Rad2Deg - 90;

    Quaternion bulletRotation = Quaternion.Euler(0f, 0f, angle);

    GameObject bullet = Instantiate(bulletPrefab, transform.position, bulletRotation);
    bullet.GetComponent<BulletController>().target = "Player";

    bullet.GetComponent<BulletController>().speed = 5f; 

    bullet.transform.up = new Vector2(idleX, idleY).normalized;

    Destroy(bullet, 2f); 

    yield return new WaitForSeconds(attackCooldown);
    isAttacking = false;
}


    public void OnAttackComplete()
    {
        // This method is called via an animation event.
        isAttacking = false;
        coolingDown = true;
        animator.SetBool("shoot", false); // Reset the Attack parameter.
        Invoke(nameof(ResetCooling), attackCooldown); // Start cooldown.
    }

    private void ResetCooling()
    {
        // Called after the cooldown period to allow the enemy to attack again.
        coolingDown = false;
        lastAttackTime = Time.time; // Reset attack time.
    }

    void Die()
    {
        animator.SetTrigger("death");
        Score.AddScore(scoreValue);
        this.enabled = false;
        StartCoroutine(FadeAlpha());
        GetComponent<Rigidbody2D>().isKinematic = true;
    }
}
