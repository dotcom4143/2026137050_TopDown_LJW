using UnityEngine;

public class ArrowInstance : MonoBehaviour
{
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifeTime = 2f;

    private float damage;
    private string element;

    public void SetupArrow(float baseDamage, string weaponElement)
    {
        damage = baseDamage;
        element = weaponElement;
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyController enemy = collision.GetComponent<EnemyController>();
        
        if (enemy != null)
        {
            enemy.TakeDamage(damage, element);
            
            Destroy(gameObject);
        }
    }
}