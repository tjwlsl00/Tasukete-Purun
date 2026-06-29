using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPlayer : MonoBehaviour
{
    private Vector2 moveInput;
    private Rigidbody2D rigidbody2D;
    private SpriteRenderer spriteRenderer;
    private float moveSpeed = 3f;

    void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // 이동 키 입력 
        moveInput.x = Input.GetAxis("Horizontal");
        moveInput.Normalize();
    }

    void FixedUpdate()
    {
        float xVelocity = moveInput.x * moveSpeed;
        rigidbody2D.linearVelocity = new Vector2(xVelocity, rigidbody2D.linearVelocity.y);

        // 플레이어 뒤집기
        if (moveInput.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveInput.x < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    #region 충돌 이벤트 게임 시작 / 종료
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("GameStart"))
        {
            StartNewGame();
            SceneManager.LoadScene("TutorialScene");
        }

        if (other.gameObject.CompareTag("ExitGame"))
        {
            Application.Quit();
        }
    }
    #endregion

    #region 스테이지3에서 저장한 데이터 삭제 / 게임 시작 시
    private void StartNewGame()
    {
        PlayerPrefs.SetInt("HasSavedPosition", 0);
        PlayerPrefs.DeleteKey("SavedSceneName");
    }
    #endregion
}
