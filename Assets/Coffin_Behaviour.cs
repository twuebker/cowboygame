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
    private Collider2D[] coll;
    public Collider2D hitBox;


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
        player = GameObject.FindGameObjectWithTag("Player").transform;
        sprite = GetComponentInChildren<SpriteRenderer>();
        coll = GetComponentsInChildren<Collider2D>();
        AIDestinationSetter aiDestinationSetter = GetComponent<AIDestinationSetter>();
        aiDestinationSetter.target = player;
    }

    public void ActivateHitbox()
    {
    // 此处hitBox是您的攻击碰撞器，可能是一个Collider2D组件
    hitBox.enabled = true;
    }

    public void DeactivateHitbox()
    {
    hitBox.enabled = false;
    }


    void Update()
    {
    if (player == null || isAttacking || coolingDown)
        {
        aiPath.canMove = false;
        return;
        }

    aiPath.canMove = true;
    UpdateAnimation(aiPath.desiredVelocity);
    CheckForAttackOpportunity();
    }

    void CheckForAttackOpportunity()
    {
    if (player == null || isAttacking || coolingDown) return;

    float distanceToPlayer = Vector3.Distance(transform.position, player.position);
    if (distanceToPlayer < attackRange && Time.time > lastAttackTime + attackCooldown)
        {   
        Attack();
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
    if (isAttacking || coolingDown) return; // 确保不重复攻击

    isAttacking = true;
    lastAttackTime = Time.time;
    aiPath.canMove = false;
    animator.SetBool("canWalk", false);
    animator.SetTrigger("Attack");

    // 假设攻击动画长度是0.6秒，我们延迟0.6秒调用OnAttackComplete
    Invoke(nameof(OnAttackComplete), 0.6f); 
}

    public void OnAttackComplete()
{
    if (!isAttacking) // 如果不是在攻击状态，不需要做任何事
    {
        return;
    }

    isAttacking = false;
    coolingDown = true;
    animator.SetBool("Attack", false);
    Invoke(nameof(ResetCooling), attackCooldown);
}

    void OnDisable()
    {
    CancelInvoke(nameof(OnAttackComplete));
    CancelInvoke(nameof(ResetCooling));
    }

    private void ResetCooling()
    {
        // Called after the cooldown period to allow the enemy to attack again.
        coolingDown = false;
        lastAttackTime = Time.time; // Reset attack time.
    }

    void Die()
    {
        if(!this.enabled) {
            return;
        }
        this.enabled = false;
        EnemySpawning.Instance.DecrementEnemyCount();
        animator.SetTrigger("death");
        Score.AddScore(scoreValue);
        foreach(Collider2D c in coll) {
            c.enabled = false;
        }
        aiPath.canMove = false;
        StartCoroutine(FadeAlpha());
    }

}
