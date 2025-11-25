using UnityEngine;
using System.Collections;

public class Mole : MonoBehaviour
{
    [Header("카메라 줌인 / 플레이어 정지")]
    // 카메라 
    private Camera camera;
    private float originalCameraSize;
    [SerializeField] float zoomCameraSize = 3f;
    [SerializeField] float zoomSpeed = 2f;
    // bool
    private bool isZoomed = false;
    // 플레이어 
    private GameObject player;

    [Header("컷신")]
    [SerializeField] CutSceneManager cutSceneManager;
    [SerializeField] GameObject Friends;
    [SerializeField] FriendDialogue friendDialogue;
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        camera = Camera.main;
        originalCameraSize = camera.orthographicSize;

        // 플레이어 
        player = GameObject.FindGameObjectWithTag("Player");
    }

    #region 충돌 이벤트 / 카메라 줌인 
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isZoomed)
        {
            // // 친구들 하트 사라지게 
            friendDialogue.UnVisibleLovePanel();

            // 컷신 시작 
            StartCoroutine(ProcessCutscene());

            // 콜라이더 비활성화 -> 중복 충돌 방지 
            GetComponent<BoxCollider2D>().enabled = false;
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

        // 카메라 줌인 
        while (Mathf.Abs(camera.orthographicSize - zoomCameraSize) > 0.01f)
        {
            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, zoomCameraSize, Time.deltaTime * zoomSpeed);

            yield return null;
        }

        cutSceneManager.StartCutScene();

    }
    #endregion

    #region 애니메이션 
    public void CryAnimPlay()
    {
        animator.SetTrigger("isCry");
    }
    #endregion

}
