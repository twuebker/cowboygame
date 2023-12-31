using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 5f;
    public string target;
    public int damage;

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
        }
        // Move the bullet in the direction it's facing
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Destroy the bullet when it hits something
        if (target == "Player")
        {
            handlePlayerCollision(other);
        }
        else if (target == "Enemy")
        {
            CoyoteController eb = other.gameObject.GetComponent<CoyoteController>();
            if (eb != null)
            {
                eb.OnHit(damage);
            }
        }
        if (other.name != "Player" && other.name != "Hitbox")
        {
            Destroy(gameObject);
        }
    }

    private void handlePlayerCollision(Collider2D other)
    {
        if (other.name == "Player" || other.name == "Hitbox")
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            PlayerController pc = player.GetComponent<PlayerController>();
            if (pc != null)
            {
                pc.Death();
            }
        }
    }
}
