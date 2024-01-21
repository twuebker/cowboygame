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
    private bool shootingCoroutineRunning = false;  

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
        Destroy(gameObject);
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
    if (shootingCoroutineRunning)
    {
        yield break; // 如果协程已经在运行，则不再执行
    }

    shootingCoroutineRunning = true;
    yield return new WaitForSeconds(0.6f); // 等待直到射击动画到达发射子弹的帧

    // 计算角色与玩家之间的方向
    Vector2 directionToPlayer = (player.position - transform.position).normalized;

    // 选择最接近的轴向方向（左、右、上、下）
    Vector2 axisAlignedDirection;
    if (Mathf.Abs(directionToPlayer.x) > Mathf.Abs(directionToPlayer.y))
    {
        // 更接近水平方向
        axisAlignedDirection = new Vector2(Mathf.Sign(directionToPlayer.x), 0);
    }
    else
    {
        // 更接近垂直方向
        axisAlignedDirection = new Vector2(0, Mathf.Sign(directionToPlayer.y));
    }

    // 子弹发射位置的偏移
    Vector2 offset = axisAlignedDirection * 0.3f;
    Vector3 bulletPosition = transform.position + new Vector3(offset.x, offset.y, 0f);

    // 计算发射角度
    float angle;
    if (axisAlignedDirection.x != 0)
    {
        // 水平方向
        angle = axisAlignedDirection.x > 0 ? 0f : 180f;
    }
    else
    {
        // 垂直方向
        angle = axisAlignedDirection.y > 0 ? 90f : -90f;
    }

    // 设置子弹的旋转角度
    Quaternion bulletRotation = Quaternion.Euler(0f, 0f, angle);

    // 实例化子弹
    GameObject bullet = Instantiate(bulletPrefab, bulletPosition, bulletRotation);
    bullet.GetComponent<BulletController>().target = "Player";
    bullet.GetComponent<BulletController>().speed = 2.5f;
    bullet.transform.up = axisAlignedDirection.normalized;

    // 销毁子弹
    Destroy(bullet, 2f);

    yield return new WaitForSeconds(attackCooldown);
    isAttacking = false;
    shootingCoroutineRunning = false;
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
    EnemySpawning.Instance.DecrementEnemyCount();
    // 停止所有移动
    if (aiPath != null)
    {
        aiPath.canMove = false; // 禁止 AIPath 控制移动
        aiPath.canSearch = false; // 禁止 AIPath 寻路
    }

    // 禁用 Rigidbody2D 组件，以防止任何物理影响
    Rigidbody2D rb = GetComponent<Rigidbody2D>();
    if (rb != null)
    {
        rb.velocity = Vector2.zero; // 将速度设置为零
        rb.isKinematic = true; // 设置为运动学，以防止物理影响
    }

    // 禁用所有碰撞器，以防止进一步的物理交互
    Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
    foreach (Collider2D collider in colliders)
    {
        collider.enabled = false;
    }
}

}
