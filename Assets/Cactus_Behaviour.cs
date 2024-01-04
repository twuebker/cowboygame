using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Cactus_Behavior : MonoBehaviour
{
    public float attackRange = 1f;
    public float attackCooldown = 1f;
    public LayerMask attackMask;
    public int hitpoints = 10;

    private AIPath aiPath;
    private Animator animator;
    private Transform player;
    private bool isAttacking = false;
    private bool coolingDown = false;
    private float lastAttackTime = 0f;

    void Start()
    {
        aiPath = GetComponent<AIPath>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (!isAttacking)
        {
            aiPath.canMove = true;
            UpdateAnimation(aiPath.desiredVelocity);
            CheckForAttackOpportunity();
        }
    }

    void CheckForAttackOpportunity()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer < attackRange && !coolingDown)
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
}
