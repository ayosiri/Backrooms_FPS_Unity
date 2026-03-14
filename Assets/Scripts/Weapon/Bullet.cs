using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float damage = 25f;

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Bullet hit: " + collision.collider.name);

        EnemyFollow enemy = collision.collider.GetComponentInParent<EnemyFollow>();

        if (enemy != null)
        {
            Debug.Log("EnemyFollow found, applying damage");
            enemy.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}