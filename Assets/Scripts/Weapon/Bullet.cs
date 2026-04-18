using UnityEngine;

// Handles collision detection and damage application for bullets
public class Bullet : MonoBehaviour
{
    public float damage = 25f;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Bullet hit: " + collision.collider.name);

        // Attempt to find an enemy component on the hit object or its parent
        EnemyFollow enemy = collision.collider.GetComponentInParent<EnemyFollow>();

        if (enemy != null)
        {
            Debug.Log("EnemyFollow found, applying damage");
            enemy.TakeDamage(damage);
        }

        // Destroy bullet after impact
        Destroy(gameObject);
    }
}
