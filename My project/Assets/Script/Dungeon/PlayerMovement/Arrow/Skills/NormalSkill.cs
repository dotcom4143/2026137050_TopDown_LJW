using UnityEngine;

public class NormalSkill : MonoBehaviour
{
    private float damage;
    [SerializeField] private float speed = 10f;

    public void Setup(float damage)
    {
        this.damage = damage;
        Destroy(gameObject, 2f);
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyController enemy = collision.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, "Normal");
            }
        }
    }
}