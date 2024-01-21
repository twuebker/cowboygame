using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballController : MonoBehaviour
{
    public Transform target;
    public float speed = 1f;

    private Vector2 previousDirection;

    void Update()
    {
        if (target != null)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            transform.position += (Vector3)direction * speed * Time.deltaTime;
            previousDirection = direction;
        }
        else
        {
            transform.position += (Vector3)previousDirection * speed * Time.deltaTime;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Player-Bullet"))
        {
            handlePlayerCollision(other);
            Destroy(gameObject); // 销毁火球
        }
    }

    private void handlePlayerCollision(Collider2D other)
    {
        PlayerController pc = other.GetComponentInParent<PlayerController>();
        if (pc == null)
        {
            pc = other.GetComponent<PlayerController>();
        }
        if (pc != null)
        {
            pc.Death();
        }
    }



}

