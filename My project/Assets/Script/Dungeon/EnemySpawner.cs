using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    [Header("필수 연결")]
    [SerializeField] private GameObject enemyPrefab;
    private Transform playerTransform;

    [Header("스폰 규칙")]
    [SerializeField] private float spawnRate = 2f;
    [SerializeField] private float minDistance = 8f;
    [SerializeField] private float maxDistance = 12f;

    private bool isSpawning = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    public void StartSpawning()
    {
        isSpawning = true;
        InvokeRepeating(nameof(SpawnEnemyAroundPlayer), 1f, spawnRate);
    }

    public void StopSpawning()
    {
        isSpawning = false;
        CancelInvoke(nameof(SpawnEnemyAroundPlayer));
    }

    private void SpawnEnemyAroundPlayer()
    {
        if (!isSpawning || playerTransform == null || enemyPrefab == null) return;

        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(minDistance, maxDistance);
        Vector3 spawnPosition = playerTransform.position + (Vector3)randomDirection * randomDistance;

        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}