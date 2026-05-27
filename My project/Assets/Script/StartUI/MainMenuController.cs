using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [Header("UI 버튼 (플, 셋, 나 순서로.)")]
    [SerializeField] private RectTransform[] menuButtons; 

    [Header("위치 (X 좌표 기준)")]
    [SerializeField] private float hideXPosition = 1200f;  // 화면 오른쪽 밖 (숨김 위치)
    [SerializeField] private float showXPosition = 600f;   // 화면 안쪽 (정착 위치)
    
    [Header("버튼 이동 속도 및 시간")]
    [SerializeField] private float slideSpeed = 8f;        // 버튼 나오는 속도
    [SerializeField] private float delayBetweenButtons = 0.15f; // 버튼 간의 등장 시차 (초 단위)

    [Header("버튼 이벤트 관련 시스템")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingButton;
    [SerializeField] private Button quitButton;

    private bool isMenuOpen = false;       // 현재 메뉴가 열려있는지 상태 체크
    private bool isAnimating = false;      // 애니메이션이 작동 중일 때는 중복 클릭 방지
    private Coroutine menuAnimationCoroutine;

    private void Start()
    {
        // 초기 세팅은 모든 버튼을 화면 오른쪽 밖으로 밀어뒀습니다.
        foreach (RectTransform btn in menuButtons)
        {
            Vector2 pos = btn.anchoredPosition;
            pos.x = hideXPosition;
            btn.anchoredPosition = pos;
        }

        // 버튼 리스너 연결
        playButton.onClick.AddListener(OnPlayClicked);
        settingButton.onClick.AddListener(OnSettingClicked);
        quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void Update()
    {
        //배경 클릭 시 토글 연출 (애니메이션 중이 아닐 때만 작동)
        if (!isAnimating && Input.GetMouseButtonDown(0))
        {
            // 만약 UI 버튼 자체를 누른 거라면 이 클릭은 무시해야 함.
            if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) return;

            ToggleMenu();
        }
    }

    private void ToggleMenu()
    {
        isMenuOpen = !isMenuOpen; // 상태 뒤집기

        if (menuAnimationCoroutine != null)
        {
            StopCoroutine(menuAnimationCoroutine);
        }

        // 상태에 따라 들어오거나 나가는 코루틴
        menuAnimationCoroutine = StartCoroutine(AnimateMenu(isMenuOpen));
    }

    private IEnumerator AnimateMenu(bool open)
    {
        isAnimating = true;

        // 목표 X 좌표 설정
        float targetX = open ? showXPosition : hideXPosition;

        /*
        등장할 때는 Play > Settings > Quit 순서로
         퇴장할 때는 역순(Quit -> Settings -> Play)으로 나가면 연출이 훨씬 고급스럽습니다. 나중에 수정하면 반박 안받음ㅇㅇ
         */
        if (open)
        {
            for (int i = 0; i < menuButtons.Length; i++)
            {
                StartCoroutine(SmoothSlide(menuButtons[i], targetX));
                yield return new WaitForSeconds(delayBetweenButtons); // 시차 주기
            }
        }
        else
        {
            for (int i = menuButtons.Length - 1; i >= 0; i--)
            {
                StartCoroutine(SmoothSlide(menuButtons[i], targetX));
                yield return new WaitForSeconds(delayBetweenButtons); // 시차 주기
            }
        }

        // 모든 버튼이 이동을 시작하고 마지막 시차까지 끝날 때까지 대기
        yield return new WaitForSeconds(0.3f); 
        isAnimating = false;
    }

    private IEnumerator SmoothSlide(RectTransform btn, float targetX)
    {
        // Lerp를 이용해 각 개별 버튼의 X 좌표만 부드럽게 이동
        while (Mathf.Abs(btn.anchoredPosition.x - targetX) > 0.5f)
        {
            Vector2 pos = btn.anchoredPosition;
            pos.x = Mathf.Lerp(pos.x, targetX, Time.deltaTime * slideSpeed);
            btn.anchoredPosition = pos;
            yield return null;
        }

        //최종 위치 강제 고정
        Vector2 finalPos = btn.anchoredPosition;
        finalPos.x = targetX;
        btn.anchoredPosition = finalPos;
    }

    // --- 버튼 기능 구현부 --- (제발 절대 건드리지 말아야하는 스크립트)
    private void OnPlayClicked()
    {
        if (isAnimating || !isMenuOpen) return;
        Debug.Log("Dungeon Entrance - Loading BaseScene");
        SceneManager.LoadScene("BaseScene");
    }

    private void OnSettingClicked()
    {
        if (isAnimating || !isMenuOpen) return;
        Debug.Log("Open Settings Window");
    }

    private void OnQuitClicked()
    {
        if (isAnimating || !isMenuOpen) return;
        Debug.Log("Quit Application");
        Application.Quit();
    }
}

// 이 스크립트는 AI 없는 자체 제작이긴 하지만, 다른 프로젝트에서 가져왔습니다.