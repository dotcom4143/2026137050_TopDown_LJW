using UnityEngine;

public class LightArrow : BaseProjectile
{
    [SerializeField] private float hitscanRange = 20f;

    protected override void Start()
    {
        ExecuteHitscan();
        Destroy(gameObject, lifeTime);
    }

    protected override void Update()
    {

    }

    private void ExecuteHitscan()
    {
        Vector2 direction = transform.right;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, hitscanRange);

        if (hit.collider != null)
        {
            EnemyController enemy = hit.collider.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, element);
            }
        }
    }
}