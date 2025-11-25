using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 게임 상태
    public enum GameManagerDireaction
    {
        Play,
        Victroy,
        Scan,
        CutScene,
        GameOver
    }
    public GameManagerDireaction GameCurrentDireaction;
    // 싱글톤
    public static GameManager Instance;
    // 슬라임 구출 카운트
    public int SaveSlimeCount;
    public int maxSaveSlimeCount = 3;
    // 씬 이름 저장 / 카메라 스캔 한 씬
    public HashSet<string> scannedStages = new HashSet<string>();

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
    }

    #region 스크립트 활성화 / 메뉴 씬 되면 게임 상태 초기화
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MenuScene")
        {
            // 구한 친구 수
            SaveSlimeCount = 0;
            // 카메라 스캔한 씬 정보
            scannedStages.Clear();
        }
    }
    #endregion

    void Start()
    {
        // 기본 상태 Play
        GameCurrentDireaction = GameManagerDireaction.Play;

        // 구출 슬라임 숫자 초기화
        SaveSlimeCount = 0;
    }

    // 게임 오버 
    public void TriggerGameOver()
    {
        if (GameCurrentDireaction == GameManagerDireaction.GameOver) return;

        GameCurrentDireaction = GameManagerDireaction.GameOver;
        Player.Instance.Dead();
    }

    public void TriggerVictroy()
    {
        GameCurrentDireaction = GameManagerDireaction.Victroy;
        UIManager.Instance.AllFriendResque();
    }

}
