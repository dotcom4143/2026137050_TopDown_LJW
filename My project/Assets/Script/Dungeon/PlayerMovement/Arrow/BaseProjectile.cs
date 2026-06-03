using UnityEngine;

public class BaseProjectile : MonoBehaviour
{
    [SerializeField] protected float speed = 15f;
    [SerializeField] protected float lifeTime = 2f;

    protected float damage;
    protected string element;

    public virtual void Setup(float baseDamage, string weaponElement)
    {
        damage = baseDamage;
        element = weaponElement;
    }

    protected virtual void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    protected virtual void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyController enemy = collision.GetComponent<EnemyController>();
        if (enemy != null)
        {
            OnHitEnemy(enemy);
        }
    }

    protected virtual void OnHitEnemy(EnemyController enemy)
    {
        enemy.TakeDamage(damage, element);
        Destroy(gameObject);
    }
}