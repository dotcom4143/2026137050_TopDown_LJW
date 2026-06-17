using UnityEngine;

public class NormalArrow : BaseProjectile
{
    [SerializeField] private float knockbackForce = 5f;

    protected override void OnHitEnemy(EnemyController enemy)
    {
        enemy.TakeDamage(damage, element);
        
        Rigidbody2D enemyRb = enemy.GetComponent<Rigidbody2D>();
        if (enemyRb != null)
        {
            Vector2 knockbackDirection = (enemy.transform.position - transform.position).normalized;
            enemyRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
        }

        Destroy(gameObject);
    }
}