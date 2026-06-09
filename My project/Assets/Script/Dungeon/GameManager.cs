using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI 연결")]
    [SerializeField] private TextMeshProUGUI timeText;

    [Header("상태 정보")]
    public float survivalTimer = 0f;
    public bool isGameActive = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (isGameActive)
        {
            survivalTimer += Time.deltaTime;

            if (timeText != null)
            {
                timeText.text = $"Time: {survivalTimer:F1}";
            }
        }
    }

    public void StartGame()
    {
        isGameActive = true;
        survivalTimer = 0f;

        // 적 스폰 시작
        if (EnemySpawner.Instance != null)
        {
            EnemySpawner.Instance.StartSpawning();
        }

        Debug.Log("게임 시작! 적들이 몰려옵니다.");
    }

    public void EndGame()
    {
        isGameActive = false;

        if (EnemySpawner.Instance != null)
        {
            EnemySpawner.Instance.StopSpawning();
        }

        if (DataManager.Instance != null)
        {
            if (survivalTimer > DataManager.Instance.data.maxSurvivalTime)
            {
                DataManager.Instance.data.maxSurvivalTime = survivalTimer;
                DataManager.Instance.SaveData();
            }
        }

        Debug.Log($"게임 종료! 기록: {survivalTimer:F2}초");
    }
}