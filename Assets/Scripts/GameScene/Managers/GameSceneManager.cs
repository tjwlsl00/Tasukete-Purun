using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    // 싱글톤
    public static GameSceneManager Instance;

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

    #region 씬 전환 재시작 / 메뉴 이동(게임 매니저 Play 상태로 전환)
    public void ReloadCurrentScene()
    {
        // 1. GameOverPanel 비활성화
        UIManager.Instance.UnVisibleGameOverPanel();

        // 2. 게임 매니저 다시 Play상태로
        GameManager.Instance.GameCurrentDireaction = GameManager.GameManagerDireaction.Play;

        // 3. 현재 씬 받아오고 로드
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void GoBackMenuScene()
    {
        // 1. GameOverPanel 비활성화
        UIManager.Instance.UnVisibleGameOverPanel();

        // 2. UIManager 오브젝트 파괴 
        if (UIManager.Instance != null)
        {
            Destroy(UIManager.Instance.gameObject);
        }
        
        // 3. 게임 매니저 다시 Play상태로
        GameManager.Instance.GameCurrentDireaction = GameManager.GameManagerDireaction.Play;

        // 4. 메뉴 씬 이동 
        SceneManager.LoadScene("MenuScene");
    }
    #endregion

}