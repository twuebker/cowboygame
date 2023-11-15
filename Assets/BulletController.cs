using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        // Move the bullet in the direction it's facing
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Destroy the bullet when it hits something
        Destroy(gameObject);
    }
}
