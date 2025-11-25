using UnityEngine;
using System.Collections;
using TMPro;

public class FriendDialogue : MonoBehaviour
{

    [Header("플레이어랑 첫 재회")]
    private Camera camera;
    private float originalCameraSize;
    [SerializeField] float zoomCameraSize = 3f;
    [SerializeField] float zoomSpeed = 2f;
    // bool 
    private bool isZoomed = false;
    // 대화창 
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] string[] sentences;
    // 하트 
    [SerializeField] GameObject LovePanel;
    // 플레이어 
    private GameObject player;


    [Header("두더지 접촉 후 이동")]
    // 타겟 위치 
    [SerializeField] Transform Target;
    [SerializeField] float moveSpeed = 3f;
    // 스프라이트 반전
    [SerializeField] SpriteRenderer friend1;
    [SerializeField] SpriteRenderer friend2;
    [SerializeField] SpriteRenderer friend3;

    void Start()
    {
        camera = Camera.main;
        originalCameraSize = camera.orthographicSize;

        // 플레이어 
        player = GameObject.FindGameObjectWithTag("Player");
    }

    #region 충돌 이벤트 / 카메라 줌인 -> 대화 시작 
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isZoomed)
        {
            StartCoroutine(ProcessCutscene());
        }
    }

    IEnumerator ProcessCutscene()
    {
        isZoomed = true;

        // 컷신 동안 플레이어 입력 무시 
        GameManager.Instance.GameCurrentDireaction = GameManager.GameManagerDireaction.CutScene;
        // 물리 영향 안받게 처리 / 재자리 고정
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        playerMovement.UnenabledMovement();

        // Vector3 targetPos = new Vector3(transform.position.x, transform.position.y, -10);

        // 카메라 줌인 
        while (Mathf.Abs(camera.orthographicSize - zoomCameraSize) > 0.01f)
        {
            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, zoomCameraSize, Time.deltaTime * zoomSpeed);

            yield return null;
        }

        // 대화 창 열기, Space 진행 / 입력까지 대기 
        dialoguePanel.SetActive(true);

        foreach (string sentence in sentences)
        {
            dialogueText.text = sentence;

            while (!Input.GetKeyDown(KeyCode.Space))
            {
                yield return null;
            }
            yield return null;
        }
        // 대화창 닫기
        dialoguePanel.SetActive(false);

        // 카메라 원상 복구 
        while (Mathf.Abs(camera.orthographicSize - originalCameraSize) > 0.01f)
        {
            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, originalCameraSize, Time.deltaTime * zoomSpeed);
            yield return null;
        }
        camera.orthographicSize = originalCameraSize;

        // 하트 이모티콘
        LovePanel.SetActive(true);

        // 플레이어 다시 움직임 가능하게 
        playerMovement.enabledMovement();

        // 게임 매니저 상태 변경
        GameManager.Instance.GameCurrentDireaction = GameManager.GameManagerDireaction.Victroy;
    }
    #endregion

    #region 하트 패널 비활성화(뒤집기) / 플레이어 쪽으로 이동
    public void UnVisibleLovePanel()
    {
        LovePanel.SetActive(false);

        // 스프라이트 뒤집기 
        friend1.flipX = true;
        friend2.flipX = true;
        friend3.flipX = true;
    }

    public void MoveToTarget()
    {
        StartCoroutine(MoveRoutine());
    }

    IEnumerator MoveRoutine()
    {
        Vector2 TargetPos = new Vector2(Target.position.x, transform.position.y);

        while (Vector2.Distance(transform.position, Target.position) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, TargetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }
    #endregion
}
