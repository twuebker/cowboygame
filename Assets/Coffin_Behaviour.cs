using UnityEngine;
using Pathfinding;
using System.Collections;


public class EnemyController : MonoBehaviour, IDamageable
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

    public float Health {
        get {
            return _health;
        }
        set {
            _health = value;
            if(_health <= 0) {
                Die();
            } else {
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
        Debug.Log("Trying to change color");
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
        player = GameObject.FindGameObjectWithTag("Player").transform;
        sprite = GetComponent<SpriteRenderer>();
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
            animator.SetFloat("MoveX", localDirection.x);
            animator.SetFloat("MoveY", localDirection.y);
            GetComponentInChildren<SpriteRenderer>().flipX = localDirection.x < 0;
        }
    }

    void Attack()
    {
        lastAttackTime = Time.time;
        isAttacking = true;
        aiPath.canMove = false;
        animator.SetBool("canWalk", false);
        animator.SetTrigger("Attack");
    }

    public void OnAttackComplete()
    {
        // This method is called via an animation event.
        isAttacking = false;
        coolingDown = true;
        animator.SetBool("Attack", false); // Reset the Attack parameter.
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
        animator.SetTrigger("Death");
        Score.AddScore(scoreValue);
        this.enabled = false;
        StartCoroutine(FadeAlpha());
        GetComponent<Rigidbody2D>().isKinematic = true;
    }

}
