using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class CoyoteController : MonoBehaviour, IDamageable
{

    private Animator animator;
    private Transform playerTransform;
    private Vector2 movement;
    private float attackTimer;
    private Vector2 startingPosition;
    private Rigidbody2D rb;
    public GameObject fireballPrefab;
    private bool canAttack = true;
    private AIPath aiPath;

    public float patrolSpeed = 2f;
    public float chaseSpeed = 3.5f;
    public float chaseTriggerDistance = 5f;
    public float attackRange = 1f;
    public float attackCooldown = 1f;
    public Vector2 patrolDistance = new Vector2(-4f, 4f);
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

    public float _health = 300;
    public int scoreValue;
    private Vector2 nextPatrolPoint;
    private bool isPatrolling = true;

    private Collider2D coll;

    private bool shotReady = false;

    private SpriteRenderer sprite;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        aiPath = GetComponent<AIPath>();
        sprite = GetComponent<SpriteRenderer>();
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        nextPatrolPoint = GetNextPatrolPoint();
        startingPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
{
    // Attack timer logic can stay in Update if it's not directly related to physics    
    if (attackTimer > 0)
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            canAttack = true;
        }
    }

    // Assuming playerTransform could be destroyed (player dies or is removed from the scene)
    if (playerTransform != null)
    {
        // Calculate distance to player and store it for use in FixedUpdate
        float distanceToPlayer = Vector2.Distance(playerTransform.position, transform.position);

        // Set the IsAttacking parameter based on the attack timer
        animator.SetBool("IsAttacking", attackTimer > 0);
    }

    // Update the sprite flip based on the AIPath's desired velocity
    if (aiPath.desiredVelocity.x < 0)
    {
        sprite.flipX = true; // left flip Sprite
    }
    else if (aiPath.desiredVelocity.x > 0)
    {
        sprite.flipX = false; // right normal Sprite
    }
}


    void FixedUpdate()
{
    if (playerTransform == null)
    {
        // If the player is not present (e.g., has been destroyed), do nothing.
        return;
    }

    // Update the AIPath's destination to be the player's position
    aiPath.destination = playerTransform.position;

    // Calculate the distance to the player again in case it's needed for logic within FixedUpdate
    float distanceToPlayer = Vector2.Distance(playerTransform.position, rb.position);

    // Perform the attack if in range and if the attack cooldown has elapsed
    if (distanceToPlayer < attackRange && canAttack)
    {
        Attack();
        canAttack = false;
        attackTimer = attackCooldown;
    }

    // The UpdateAnimation call will use the AIPath's desired velocity to update the animation parameters
    UpdateAnimation(aiPath.desiredVelocity);
}

void UpdateAnimation(Vector2 velocity)
{
    // Check if the AIPath component is moving and if the velocity is significant
    bool isMoving = aiPath.canMove && velocity.sqrMagnitude > 0.1f;
    animator.SetBool("IsMoving", isMoving); // Use the correct parameter name as defined in your Animator
    
    if (isMoving)
    {
        // Calculate the local direction of the movement
        Vector2 localDirection = transform.InverseTransformDirection(velocity.x, velocity.y, 0);
        // Set the MoveX and MoveY parameters with the local direction values
        animator.SetFloat("MoveX", localDirection.x);
        animator.SetFloat("MoveY", localDirection.y);
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
        if (playerTransform == null)
        {
            return;
        }

        isPatrolling = false;
        MoveTowards(playerTransform.position, chaseSpeed);
    }

    void MoveTowards(Vector2 target, float speed)
    {
        if (target == null)
        {
            return;
        }
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
        StartCoroutine(SpawnFireball());
    }

    void Die()
{
    if (!this.enabled)
    {
        return;
    }

    // 触发死亡动画
    animator.SetTrigger("Death");

    // 增加分数
    Score.AddScore(scoreValue);

    // 开始透明度渐变协程
    StartCoroutine(FadeAlpha());

    // 禁用碰撞器
    coll.enabled = false;

    // 禁用移动和攻击脚本
    this.enabled = false;

    // 停止所有运动，防止死亡时移动
    if (rb != null)
    {
        rb.velocity = Vector2.zero;
        rb.isKinematic = true; // 使其变为运动学，防止受到物理影响
    }

    // 禁用 AIPath 组件，防止 AI 控制
    if (aiPath != null)
    {
        aiPath.enabled = false;
    }
}


    // This method will be called by an Animation Event during the attack animation.
    public IEnumerator SpawnFireball()
    {
        if (fireballPrefab != null)
        {
            while (!shotReady)
            {
                yield return null;
            }
            shotReady = false;
            GameObject fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);
            FireballController fireballController = fireball.GetComponent<FireballController>();
            Destroy(fireball, 3f);
            if (fireballController != null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player == null)
                {
                    Debug.Log("No player!");
                    yield break;
                }
                Transform t = player.transform;
                fireballController.target = t;
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

    public void SetShotReady()
    {
        shotReady = true;
    }

    public void Destroy()
    {
        Destroy(gameObject);
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
}
