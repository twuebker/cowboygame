using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Behavior : MonoBehaviour
{
    private Animator animator;
    private Transform playerTransform;
    private Vector2 movement;
    private float attackTimer;
    private Vector2 startingPosition;
    private Rigidbody2D rb;
    public GameObject fireballPrefab;
    private bool canAttack = true;



    public float patrolSpeed = 2f;
    public float chaseSpeed = 3.5f;
    public float chaseTriggerDistance = 5f;
    public float attackRange = 1f;
    public float attackCooldown = 1f;
    public Vector2 patrolDistance = new Vector2(-4f, 4f);

    private Vector2 nextPatrolPoint;
    private bool isPatrolling = true;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        nextPatrolPoint = GetNextPatrolPoint();
        startingPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (attackTimer > 0)
        {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
            {
            canAttack = true; 
            }
        }

    // Attack timer logic can stay in Update if it's not directly related to physics    
    if (attackTimer > 0)
        {
        attackTimer -= Time.deltaTime;
        }

    // Update animator parameters
    animator.SetFloat("MoveX", Mathf.Abs(movement.x));
    animator.SetFloat("MoveY", movement.y);
    animator.SetBool("IsMoving", movement != Vector2.zero);
    animator.SetBool("IsAttacking", attackTimer > 0);

    // Sprite flipping logic can stay in Update
    if (movement.x < 0)
        {
        GetComponent<SpriteRenderer>().flipX = true; // left flip Sprite
        }
    else if (movement.x > 0)
        {
        GetComponent<SpriteRenderer>().flipX = false; // right normal Sprite
        }
    }

    void FixedUpdate()
    {
    float distanceToPlayer = Vector2.Distance(playerTransform.position, rb.position);

    if (distanceToPlayer < attackRange && canAttack)
        {
        Attack();
        canAttack = false; 
        attackTimer = attackCooldown; 
        }
        else if (distanceToPlayer < chaseTriggerDistance)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }

    }

    void Patrol()
    {
        if ((Vector2)transform.position == nextPatrolPoint || isPatrolling == false)
        {
            nextPatrolPoint = GetNextPatrolPoint();
            isPatrolling = true;
        }

        MoveTowards(nextPatrolPoint, patrolSpeed);
    }

    void ChasePlayer()
    {
        isPatrolling = false;
        MoveTowards(playerTransform.position, chaseSpeed);
    }

    void MoveTowards(Vector2 target, float speed)
    {
    movement = (target - (Vector2)transform.position).normalized * speed;
    //  Rigidbody2D.MovePosition to Transform.position
    rb.MovePosition(Vector2.MoveTowards(rb.position, target, speed * Time.deltaTime));
    }


    Vector2 GetNextPatrolPoint()
    {
        // Assuming the enemy patrols on a horizontal path
        return new Vector2(startingPosition.x + Random.Range(patrolDistance.x, patrolDistance.y), startingPosition.y);
    }

    void Attack()
    {
        // Reset attack timer
        attackTimer = attackCooldown;

        // Perform the attack
        Debug.Log("Coyote attacks the player!");

        // Trigger attack animation
        animator.SetTrigger("Attack");

        // The actual spawning of the fireball is now handled by the SpawnFireball method,
        // which will be called by an Animation Event.
    }

// This method will be called by an Animation Event during the attack animation.
    public void SpawnFireball()
    {
        if (fireballPrefab != null)
        {
            GameObject fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);
            FireballController fireballController = fireball.GetComponent<FireballController>();
            if (fireballController != null)
            {
                fireballController.target = GameObject.FindGameObjectWithTag("Player").transform;
            }
            else
            {
                Debug.LogError("FireballController component not found on the fireball prefab!");
            }
        }
        else
        {
            Debug.LogError("Fireball prefab not assigned!");
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            Die();
        }
    }

    void Die()
    {
        // Trigger death animation
        animator.SetTrigger("Death");

        // Disable enemy components or behaviors that should not function after death
        // For example, disable the script that controls movement and attacks
        this.enabled = false; 

        // Optionally wait for the death animation to finish playing before destroying the object
        // This will require a coroutine since we need to delay the destruction
        StartCoroutine(DestroyAfterAnimation(animator));
    }

    IEnumerator DestroyAfterAnimation(Animator animator)
    {
    // Wait until the current animation clip is mostly finished playing
    // You can multiply the length by a fraction (like 0.8f) to wait for less than the full length
    float waitTime = animator.GetCurrentAnimatorStateInfo(0).length * 0.52f;
    yield return new WaitForSeconds(waitTime);

    // Then destroy the enemy game object
    Destroy(gameObject);
    }

}