using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float collisionOffset = 0.05f;
    public ContactFilter2D movementFilter;

    bool canMove = true;

    bool shotReady = false;

    public GameObject bulletPrefab;

    public Transform firePoint;

    Vector2 movementInput;
    Rigidbody2D rb;
    Animator animator;
    SpriteRenderer spriteRenderer;
    List<RaycastHit2D> castCollisions = new List<RaycastHit2D>();

    AudioSource shotAudio;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        shotAudio = GetComponent<AudioSource>();
    }

    private void FixedUpdate()
    {
        if(canMove) 
        {
            if(movementInput != Vector2.zero) 
            {
                bool success = TryMove(movementInput);
                if(!success) 
                {
                    success = TryMove(new Vector2(movementInput.x, 0));
                    if(!success) {
                        success = TryMove(new Vector2(0, movementInput.y));
                    }
                }
                animator.SetBool("isMoving", success);
            } else {
                animator.SetBool("isMoving", false);
            }
            handleAnimations();
            if(movementInput.x < 0) {
                spriteRenderer.flipX = true;
            } else if(movementInput.x > 0) {
                spriteRenderer.flipX = false;
            }
        }
    }

    private void handleAnimations() {
        if(movementInput != Vector2.zero) {
            animator.SetFloat("moveX", movementInput.x);
            animator.SetFloat("moveY", movementInput.y);
            animator.SetFloat("idleX", movementInput.x);
            animator.SetFloat("idleY", movementInput.y);
        }
    }

    private bool TryMove(Vector2 direction) {
        if(direction == Vector2.zero) {
            return false;
        }
        int count = rb.Cast(
                direction,
                movementFilter,
                castCollisions,
                moveSpeed * Time.fixedDeltaTime + collisionOffset);
        if(count == 0) 
        {
            rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
            return true;
        } else 
        {
            return false;
        }
    }

    void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
    }

    void OnFire() 
    {
        animator.SetTrigger("shot");
        StartCoroutine(Shoot());
    }

    public void LockMovement()
    {
        canMove = false;
    }

    public void UnlockMovement()
    {
        canMove = true;
    }

    public void ShotAnimationReady()
    {
        shotReady = true;
    }

    public IEnumerator Shoot()
    {
        float idleX = animator.GetFloat("idleX");
        float idleY = animator.GetFloat("idleY");
        float angle = Mathf.Atan2(idleY, idleX) * Mathf.Rad2Deg - 90;

        // Set the rotation of the bullet based on the angle
        Quaternion bulletRotation = Quaternion.Euler(0f, 0f, angle);
        // Instantiate a bullet at the fire point
        while (!shotReady)
        {
            yield return null;
        }
        Vector3 pos = firePoint.position;
        //correct some weird looking shots
        if(idleX>0) {
            pos.y += 0.02f;
        }
        //play shot audio
        shotAudio.Play();
        GameObject bullet = Instantiate(bulletPrefab, pos, bulletRotation);
        shotReady = false;
        // Attach bullet controller script to the bullet
        BulletController bulletController = bullet.GetComponent<BulletController>();
        
        // Set bullet speed
        bulletController.speed = 3f;
        Destroy(bullet, 2f);
    }

    public void Death() {
        
    }
}
