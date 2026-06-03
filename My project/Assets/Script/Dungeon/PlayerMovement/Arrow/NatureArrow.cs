using UnityEngine;

public class NatureArrow : BaseProjectile
{
    [SerializeField] private GameObject vinePrefab;
    [SerializeField] private float spawnInterval = 0.2f;
    
    private float spawnTimer;

    protected override void Update()
    {
        base.Update();

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            SpawnVine();
        }
    }

    private void SpawnVine()
    {
        if (vinePrefab != null)
        {
            GameObject vine = Instantiate(vinePrefab, transform.position, Quaternion.identity);
            Destroy(vine, 5f);
        }
    }
}