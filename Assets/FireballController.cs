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
    // 检查碰撞对象是否为玩家或其子弹
    if(other.CompareTag("Player") || other.CompareTag("Player-Bullet"))
    {
        handlePlayerCollision(other);
        Destroy(gameObject); // 销毁火球
    }
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

