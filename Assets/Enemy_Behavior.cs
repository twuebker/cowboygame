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
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector2.Distance(playerTransform.position, transform.position);

        if (distanceToPlayer < attackRange && attackTimer <= 0)
        {
            Attack();
        }
        else if (distanceToPlayer < chaseTriggerDistance)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }

        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        // Update animator parameters
        animator.SetFloat("MoveX", Mathf.Abs(movement.x));
        animator.SetFloat("MoveY", movement.y);
        animator.SetBool("IsMoving", movement != Vector2.zero);
        animator.SetBool("IsAttacking", attackTimer > 0);

        if (movement.x < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true; // left flip Sprite
        }
        else if (movement.x > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false; // right normal Sprite
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
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
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
    }   
}
