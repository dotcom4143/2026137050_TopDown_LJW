using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    [Header("필수 연결")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private List<MonsterData> monsterDataList;

    [Header("스폰 규칙")]
    [SerializeField] private float spawnRate = 2f;
    [SerializeField] private float minDistance = 8f;
    [SerializeField] private float maxDistance = 12f;

    private Transform playerTransform;
    private bool isSpawning = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        FindPlayer();
        StartSpawning();
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
            Debug.LogWarning("EnemySpawner: 'Player' 태그를 가진 오브젝트가 안보임;;");
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
        if (monsterDataList == null || monsterDataList.Count == 0)
        {
            Debug.LogWarning("EnemySpawner: 스폰할 몬스터가 리스트에 없슨;;");
            return;
        }

        int randomIndex = Random.Range(0, monsterDataList.Count);
        MonsterData selectedData = monsterDataList[randomIndex];

        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomDistance = Random.Range(minDistance, maxDistance);
        Vector3 spawnPosition = playerTransform.position + (Vector3)randomDirection * randomDistance;
        spawnPosition.z = 0;

        GameObject enemyObj = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        enemyObj.GetComponent<EnemyController>()?.Setup(selectedData);
    }
}