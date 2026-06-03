using UnityEngine;

public class DarknessArrow : BaseProjectile
{
    [SerializeField] private float detectionRadius = 10f;
    
    private Transform targetEnemy;

    protected override void Start()
    {
        base.Start();
        FindClosestEnemy();
    }

    protected override void Update()
    {
        if (targetEnemy != null)
        {
            Vector3 direction = (targetEnemy.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    private void FindClosestEnemy()
    {
        Collider2D[] results = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D col in results)
        {
            EnemyController enemy = col.GetComponent<EnemyController>();
            if (enemy != null)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    targetEnemy = enemy.transform;
                }
            }
        }
    }
}