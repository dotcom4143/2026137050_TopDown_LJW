using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Animation Sprites")]
    public Sprite[] spriteUp;
    public Sprite[] spriteDown;
    public Sprite[] spriteLeft;
    public Sprite[] spriteRight;

    public float frameTime = 0.15f;

    [Header("Player Health System")]
    public float maxHealth = 100f;
    private float currentHealth;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Vector2 input;
    private Vector2 velocity;
    private Sprite[] currentSprites;
    private int frameIndex = 0;
    private float timer = 0f;

    [System.Serializable]
    private class UpgradeData
    {
        public int playerHealthLevel = 0;
        public int moveSpeedLevel = 0;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        currentSprites = spriteDown;
        sr.sprite = currentSprites[0];
    }

    private void Start()
    {
        ApplyJsonUpgrades();
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if (input.sqrMagnitude <= 0.01f)
        {
            frameIndex = 0;
            sr.sprite = currentSprites[frameIndex];
            return;
        }

        timer += Time.deltaTime;

        if (timer >= frameTime)
        {
            timer = 0f;
            frameIndex++;

            if (frameIndex >= currentSprites.Length)
                frameIndex = 0;

            sr.sprite = currentSprites[frameIndex];
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }

    private void ChangeSprites(Sprite[] newSprites)
    {
        if (currentSprites == newSprites)
            return;

        currentSprites = newSprites;
        frameIndex = 0;
        timer = 0f;
        sr.sprite = currentSprites[frameIndex];
    }

    public void OnMove(InputValue value)
    {
        input = value.Get<Vector2>();
        velocity = input.normalized * moveSpeed;

        if (input.sqrMagnitude > 0.01f)
        {
            if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
            {
                if (input.x > 0)
                    ChangeSprites(spriteRight);
                else
                    ChangeSprites(spriteLeft);
            }
            else
            {
                if (input.y > 0)
                    ChangeSprites(spriteUp);
                else
                    ChangeSprites(spriteDown);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"[플레이어 피격] 데미지: {damage} | 남은 체력: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("플레이어가 사망했습니다!");

        ReturnToMainMenu uiController = FindFirstObjectByType<ReturnToMainMenu>();
        if (uiController != null)
        {
            uiController.OnPlayerDead();
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.EndGame();
        }

        velocity = Vector2.zero;
        input = Vector2.zero;
        gameObject.SetActive(false); 
    }

    private void ApplyJsonUpgrades()
    {
        string savePath = Path.Combine(Application.persistentDataPath, "SaveData.json");

        if (File.Exists(savePath))
        {
            try
            {
                string json = File.ReadAllText(savePath);
                UpgradeData data = JsonUtility.FromJson<UpgradeData>(json);

                maxHealth = maxHealth * Mathf.Pow(1.1f, data.playerHealthLevel);
                moveSpeed = moveSpeed * Mathf.Pow(1.1f, data.moveSpeedLevel);

                Debug.Log($"[스탯 강화 적용 완료] 체력 LV.{data.playerHealthLevel} -> 최종 체력: {maxHealth}");
                Debug.Log($"[스탯 강화 적용 완료] 이속 LV.{data.moveSpeedLevel} -> 최종 이속: {moveSpeed}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[강화 로드 실패] JSON 해석 중 오류 발생: {e.Message}");
            }
        }
        else
        {
            Debug.Log("[스탯 강화] 세이브 파일이 없어 기본 능력치로 시작합니다.");
        }
    }
}