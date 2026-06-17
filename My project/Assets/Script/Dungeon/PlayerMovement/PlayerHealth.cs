using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float currentHealth = 100f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            TakeDamage(10f);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"플레이어 피격. 남은 체력: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("게임 오버");

        ReturnToMainMenu uiController = FindFirstObjectByType<ReturnToMainMenu>();
        if (uiController != null)
        {
            uiController.OnPlayerDead();
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.EndGame();
        }
    }
}