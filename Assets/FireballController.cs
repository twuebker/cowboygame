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
        } else {
            transform.position += (Vector3)previousDirection * speed * Time.deltaTime;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.name == "Enemy") {
            return;
        }
        handlePlayerCollision(other);
        Destroy(gameObject);
    }

    private void handlePlayerCollision(Collider2D other) {
        // Destroy the fireball when it hits something
        if(other.name == "Player" || other.name == "Hitbox") {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            PlayerController pc = player.GetComponent<PlayerController>();
            if(pc != null) {
                pc.Death();
            }
        }
    }
}

