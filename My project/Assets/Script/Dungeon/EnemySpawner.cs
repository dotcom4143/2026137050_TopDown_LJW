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
        FindPlayer();
    }

    private void FindPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("EnemySpawner: 'Player' 태그를 가진 오브젝트를 찾을 수 없습니다.");
        }
    }

    public void StartSpawning()
    {
        if (playerTransform == null) FindPlayer();

        isSpawning = true;
        InvokeRepeating(nameof(SpawnEnemy), 1f, spawnRate);
    }

    public void StopSpawning()
    {
        isSpawning = false;
        CancelInvoke(nameof(SpawnEnemy));
    }

    private void SpawnEnemy()
    {
        if (!isSpawning || playerTransform == null || enemyPrefab == null) return;

        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(minDistance, maxDistance);
        Vector3 spawnPosition = playerTransform.position + (Vector3)randomDirection * randomDistance;

        Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
    }
}