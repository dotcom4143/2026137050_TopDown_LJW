using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ReturnToMainMenu : MonoBehaviour
{
    [Header("게임 오버 패널")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("시작 화면으로 돌아가는 버튼")]
    [SerializeField] private Button returnButton;

    [Header("메인 메뉴 씬 이름")]
    [SerializeField] private string mainMenuSceneName = "StartScene";

    private void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (returnButton != null)
        {
            returnButton.onClick.AddListener(OnReturnButtonClicked);
        }
    }

    public void OnPlayerDead()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    private void OnReturnButtonClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}