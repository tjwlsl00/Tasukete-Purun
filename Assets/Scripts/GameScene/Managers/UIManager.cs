using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEditor;
using TMPro;

public class UIManager : MonoBehaviour
{
    private Color activeColor = Color.white;
    private Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 0.8f);
    private int currentSelectedIndex = 0;
    private int currentSavedFriend = 0;
    // UI 연결
    [SerializeField] Image[] slimeIcons;
    [SerializeField] Image[] Prisons;
    [SerializeField] GameObject ResquePanel;
    [SerializeField] GameObject AllResquePanel;
    [SerializeField] GameObject GameOverPanel;
    [SerializeField] GameObject MapScan;
    [SerializeField] GameObject MenuPanel;
    [SerializeField] GameObject LeftInfoPanel;
    private TextMeshProUGUI LeftInfoText;
    [SerializeField] FadeEffect fadeEffect;
    [SerializeField] AudioClip ButtonClip;
    // 컴포넌트 
    private AudioSource audioSource;
    // 싱글톤 
    public static UIManager Instance;
    // 외부 스크립트 
    private Player player;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // 컴포넌트
        audioSource = GetComponent<AudioSource>();

        // 외부 스크립트 
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    void Start()
    {
        ResquePanel.SetActive(true);
        AllResquePanel.SetActive(false);
        MapScan.SetActive(false);
        LeftInfoPanel.SetActive(false);
        LeftInfoText = LeftInfoPanel.GetComponent<TextMeshProUGUI>();

        // 게임 오버 패널 비활성화
        UnVisibleGameOverPanel();

        // 메뉴 패널 비활성화 
        MenuPanel.SetActive(false);

        // 슬라임 구출 카운트 업데이트 
        UpdateSaveSlimeUI(-1);
    }

    void Update()
    {
        // 슬라임 변신 효과
        if (GameManager.Instance.GameCurrentDireaction == GameManager.GameManagerDireaction.Play)
        {
            if (player != null)
            {
                bool isPlayerStanding = player.CanStandup();

                if (isPlayerStanding)
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1))
                    {
                        SelectSlot(0);
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha2))
                    {
                        SelectSlot(1);
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha3))
                    {
                        SelectSlot(2);
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha4))
                    {
                        SelectSlot(3);
                    }
                }
            }
        }

        // 메뉴패널 토글
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleMenuPanel();
        }
    }

    #region 스크립트 활성화 / 페이드 인 함수 호출, 맵 스캔 UI
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 맵 스캔 UI
        if (scene.name == "TutorialScene" || scene.name == "FinalScene")
        {
            MapScan.SetActive(false);
        }
        else
        {
            MapScan.SetActive(true);
        }

        // 페이드 인 함수 호출
        if (fadeEffect != null)
        {
            fadeEffect.StartFadeIn();
        }

        // 초기 상태 : 슬롯 1 (초록색)
        SelectSlot(0);
    }
    #endregion

    #region 슬라임 전환 효과
    void SelectSlot(int index)
    {
        if (index < 0 || index >= slimeIcons.Length)
        {
            return;
        }
        currentSelectedIndex = index;

        for (int i = 0; i < slimeIcons.Length; i++)
        {
            if (i == currentSelectedIndex)
            {
                slimeIcons[i].color = activeColor;
            }
            else
            {
                slimeIcons[i].color = inactiveColor;
            }
        }
    }
    #endregion

    #region 게임 오버 UI 켜기 / 끄기
    public void VisibleGameOverPanel()
    {
        GameOverPanel.SetActive(true);
    }

    public void UnVisibleGameOverPanel()
    {
        GameOverPanel.SetActive(false);
    }
    #endregion

    #region 메뉴 패널 UI 토글
    public void ToggleMenuPanel()
    {
        if (MenuPanel != null)
        {
            bool isActive = !MenuPanel.activeSelf;
            MenuPanel.SetActive(isActive);
        }
    }
    #endregion

    #region 슬라임 구출 UI 업데이트  / 남은 친구 정보 / 감옥 페이드 아웃
    public void UpdateSaveSlimeUI(int savedFriend)
    {
        currentSavedFriend = savedFriend;

        for (int i = 0; i < Prisons.Length; i++)
        {
            Image prisonImage = Prisons[i].GetComponent<Image>();

            if (i < currentSavedFriend)
            {
                if (Prisons[i].gameObject.activeInHierarchy)
                {
                    // 이미지 컴포넌트 전달
                    StartCoroutine(FadeOutPrison(prisonImage));
                    StartCoroutine(LeftInfo(currentSavedFriend));
                }
            }
            else
            {
                Prisons[i].gameObject.SetActive(true);

                // 투명도 다시 원상복구 
                if (prisonImage != null)
                {
                    Color c = prisonImage.color;
                    c.a = 1f;
                    prisonImage.color = c;
                }
            }
        }
    }

    IEnumerator LeftInfo(int currentSavedFriend)
    {
        LeftInfoPanel.SetActive(true);

        // 수치 계산 
        int maxCount = GameManager.Instance.maxSaveSlimeCount;
        int leftCount = maxCount - currentSavedFriend;

        if (currentSavedFriend > 0 && currentSavedFriend < maxCount)
        {
            LeftInfoText.text = "残りのお友達：" + leftCount;
        }
        else if(currentSavedFriend == maxCount)
        {
            LeftInfoText.text = "全員救いました！";
        }

        LeftInfoText.canvasRenderer.SetAlpha(2.0f);
        LeftInfoText.CrossFadeAlpha(0.0f, 1.0f, false);

        yield return new WaitForSeconds(2.0f);

        LeftInfoPanel.SetActive(false);
    }

    // 감옥 페이드 아웃으로 서서히 사라지게 
    IEnumerator FadeOutPrison(Image targetImage)
    {
        if (targetImage == null) yield break;

        float duration = 0.5f;
        float currentTime = 0f;
        Color startColor = targetImage.color;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            // Mathf.Lerp(시작값, 중간값, 진행률);
            // 시간 점차 증가 -> 진행률 증가 
            float alpha = Mathf.Lerp(1f, 0f, currentTime / duration);

            targetImage.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        // 투명해지면 완전히 안보이게 처리 
        targetImage.gameObject.SetActive(false);
    }

    // 구출 이미지 페이드 인 
    public void AllFriendResque()
    {
        ResquePanel.SetActive(false);
        AllResquePanel.SetActive(true);

        Image AllResqueImage = AllResquePanel.GetComponent<Image>();

        if (AllResqueImage != null)
        {
            AllResqueImage.canvasRenderer.SetAlpha(0.0f);
            AllResqueImage.CrossFadeAlpha(1.0f, 0.5f, false);
        }
    }
    #endregion

    #region 버튼 사운드 
    public void PlayButtonClip()
    {
        audioSource.clip = ButtonClip;
        audioSource.Play();
    }
    #endregion
}
