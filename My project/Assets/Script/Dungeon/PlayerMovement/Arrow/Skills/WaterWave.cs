using UnityEngine;

public class WaterWave : MonoBehaviour
{
    private float speed = 10f;
    private float knockbackForce = 15f;
    private float damage;

    public void Setup(Vector3 direction, float damage)
    {
        this.damage = damage;
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * speed;
        }
        Destroy(gameObject, 2f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Rigidbody2D enemyRb = collision.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
                enemyRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
            }
            EnemyController enemy = collision.GetComponent<EnemyController>();
            if (enemy != null) enemy.TakeDamage(damage, "Water");
        }
    }
}