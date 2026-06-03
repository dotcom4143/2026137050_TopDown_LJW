using UnityEngine;

public class WaterArrow : BaseProjectile
{
    [SerializeField] private float splashRadius = 1.5f;

    protected override void OnHitEnemy(EnemyController enemy)
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, splashRadius);
        
        foreach (Collider2D col in hitEnemies)
        {
            EnemyController target = col.GetComponent<EnemyController>();
            if (target != null)
            {
                target.TakeDamage(damage, element);
            }
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, splashRadius);
    }
}