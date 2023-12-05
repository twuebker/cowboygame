using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballController : MonoBehaviour
{
    public Transform target;
    public float speed = 5f;

    void Update()
    {
        if (target != null)
        {
            Vector2 direction = (target.position - transform.position).normalized;
            transform.position += (Vector3)direction * speed * Time.deltaTime;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Destroy the fireball when it hits something
        Debug.Log("HELLO WORLD" + other.name);
        if(other.name == "Player") {
            PlayerController pc = other.gameObject.GetComponent<PlayerController>();
            if(pc != null) {
                pc.Death();
            }
        }
        if(other.name != "Enemy") {
            Destroy(gameObject);
        }
    }
}

