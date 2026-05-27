using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private Sprite[] enemySprites;
    [SerializeField] private AudioClip deathSound;

    private Transform playerTransform;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && enemySprites != null && enemySprites.Length > 0)
        {
            int randomIndex = Random.Range(0, enemySprites.Length);
            spriteRenderer.sprite = enemySprites[randomIndex];
        }

        Invoke("DieAndDropCoin", 5f);
    }

    private void Update()
    {
        if (playerTransform != null)
        {
            Vector3 direction = (playerTransform.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }

    private void DieAndDropCoin()
    {
        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position);
        }

        if (coinPrefab != null)
        {
            Instantiate(coinPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}