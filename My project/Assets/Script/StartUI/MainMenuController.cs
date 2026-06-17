using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("UI 버튼 (플, 셋, 나 순서로.)")]
    [SerializeField] private RectTransform[] menuButtons; 

    [Header("위치 (X 좌표 기준)")]
    [SerializeField] private float hideXPosition = 1200f;  
    [SerializeField] private float showXPosition = 600f;   
    
    [Header("버튼 이동 속도 및 시간")]
    [SerializeField] private float slideSpeed = 8f;        
    [SerializeField] private float delayBetweenButtons = 0.15f; 

    [Header("버튼 이벤트 관련 시스템")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button quitButton;

    [Header("배경 패널 및 세팅 컨트롤러")]
    [SerializeField] private Button backgroundPanel; 
    [SerializeField] private SettingsMenuController settingsMenu;

    private bool isMenuOpen = false;       
    private bool isAnimating = false;      
    private Coroutine menuAnimationCoroutine;

    private void Start()
    {
        if (backgroundPanel != null)
        {
            backgroundPanel.onClick.AddListener(OnBackgroundPanelClicked);
        }

        if (settingsMenu != null)
        {
            settingsMenu.gameObject.SetActive(false);
        }

        foreach (RectTransform btn in menuButtons)
        {
            Vector2 pos = btn.anchoredPosition;
            pos.x = hideXPosition;
            btn.anchoredPosition = pos;
        }

        playButton.onClick.AddListener(OnPlayClicked);
        settingButton.onClick.AddListener(OnSettingClicked);
        quitButton.onClick.AddListener(OnQuitClicked);

        if (backgroundPanel != null)
        {
            backgroundPanel.onClick.AddListener(OnBackgroundPanelClicked);
        }
    }

    private void OnBackgroundPanelClicked()
    {
        if (isAnimating) return;
        ToggleMenu();
    }

    private void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen; 
        if (menuAnimationCoroutine != null) StopCoroutine(menuAnimationCoroutine);
        menuAnimationCoroutine = StartCoroutine(AnimateMenu(isMenuOpen));
    }

    private IEnumerator AnimateMenu(bool open)
    {
        isAnimating = true;
        float targetX = open ? showXPosition : hideXPosition;

        if (open)
        {
            for (int i = 0; i < menuButtons.Length; i++)
            {
                StartCoroutine(SmoothSlide(menuButtons[i], targetX));
                yield return new WaitForSeconds(delayBetweenButtons); 
            }
        }
        else
        {
            for (int i = menuButtons.Length - 1; i >= 0; i--)
            {
                StartCoroutine(SmoothSlide(menuButtons[i], targetX));
                yield return new WaitForSeconds(delayBetweenButtons); 
            }
        }

        yield return new WaitForSeconds(0.3f); 
        isAnimating = false;
    }

    private IEnumerator SmoothSlide(RectTransform btn, float targetX)
    {
        while (Mathf.Abs(btn.anchoredPosition.x - targetX) > 0.5f)
        {
            Vector2 pos = btn.anchoredPosition;
            pos.x = Mathf.Lerp(pos.x, targetX, Time.deltaTime * slideSpeed);
            btn.anchoredPosition = pos;
            yield return null;
        }
        Vector2 finalPos = btn.anchoredPosition;
        finalPos.x = targetX;
        btn.anchoredPosition = finalPos;
    }

    private void OnPlayClicked()
    {
        if (isAnimating || !isMenuOpen) return;
        SceneManager.LoadScene("BaseScene");
    }

    private void OnSettingClicked()
    {
        if (isAnimating || !isMenuOpen) return;
        
        if (settingsMenu != null)
        {
            settingsMenu.OpenPanel();
        }
    }

    private void OnQuitClicked()
    {
        if (isAnimating || !isMenuOpen) return;
        Application.Quit();
    }
}