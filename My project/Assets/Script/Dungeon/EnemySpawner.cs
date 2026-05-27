using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("필수 연결")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Transform playerTransform;

    [Header("스폰 규칙")]
    [SerializeField] private float spawnRate = 2f;
    [SerializeField] private float minDistance = 8f;
    [SerializeField] private float maxDistance = 12f;

    private float nextSpawnTime;

    private void Update()
    {
        if (playerTransform == null || enemyPrefab == null) return;

        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemyAroundPlayer();
            nextSpawnTime = Time.time + spawnRate;
        }
    }

    private void SpawnEnemyAroundPlayer()
    {
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        
        float randomDistance = Random.Range(minDistance, maxDistance);
        
        Vector3 spawnPosition = playerTransform.position + new Vector3(randomDirection.x, randomDirection.y, 0) * randomDistance;

        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}