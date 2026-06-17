using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private AudioClip collectSound;
    [SerializeField] private int coinValue = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"[코인 충돌] 부딪힌 오브젝트: {collision.name} | 태그: {collision.tag}");

        if (collision.CompareTag("Player"))
        {
            if (collectSound != null)
            {
                AudioSource.PlayClipAtPoint(collectSound, transform.position);
            }

            if (GameManager.Instance != null)
            {
                DataManager.Instance.AddCoin(coinValue);
            }

            if (DataManager.Instance != null)
            {
                DataManager.Instance.AddCoin(coinValue);
            }

            Destroy(gameObject);
        }
    }

}