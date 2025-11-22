using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
    public enum PlayerType
    {
        Default,
        Red,
        Yellow,
        Blue
    }
    public PlayerType currentPlayerType;

    // 싱글톤
    public static Player Instance;
    public static Transform playerTransform;
    // 외부 스크립트  
    private FollowPlayer followPlayer;
    private PlayerMovement playerMovement;
    private PlayerAnimator playerAnimator;
    private PlayerAudio playerAudio;
    private RedAbility redAbility;
    private YellowAbility yellowAbility;
    private BlueAbility blueAbility;
    [SerializeField] GameObject ChangeEffect;
    private ChangeEffect changeEffect;
    public Vector2 moveInput;
    // bool 
    private bool isDead;
    // 장애물 체크 
    private GameObject CheckBox;
    private Transform CheckBoxTransform;
    public Vector2 checkSize;
    [SerializeField] private LayerMask groundLayer;

    void Awake()
    {
        // 싱글톤
        if (Instance == null)
        {
            Instance = this;
            playerTransform = transform;
        }
        else
        {
            Destroy(gameObject);
        }

        // 외부 스크립트
        followPlayer = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<FollowPlayer>();
        playerMovement = GetComponent<PlayerMovement>();
        playerAnimator = GetComponent<PlayerAnimator>();
        playerAudio = GetComponent<PlayerAudio>();
        redAbility = GetComponent<RedAbility>();
        yellowAbility = GetComponent<YellowAbility>();
        blueAbility = GetComponent<BlueAbility>();
        changeEffect = ChangeEffect.GetComponent<ChangeEffect>();

        // 오브젝트 연결
        CheckBox = GameObject.Find("CheckBox");
        CheckBoxTransform = CheckBox.transform;
    }

    void Start()
    {
        // 플레이어 상태 초기화
        currentPlayerType = PlayerType.Default;
        isDead = false;

        // 모든 능력 일단 비활성화
        DisableAllAbilities();
    }

    void Update()
    {
        #region 살아있을 때
        if (!isDead && GameManager.Instance.GameCurrentDireaction == GameManager.GameManagerDireaction.Play || GameManager.Instance.GameCurrentDireaction == GameManager.GameManagerDireaction.Victroy)
        {
            #region 키 입력 이동 / 점프(초록, 빨강)
            // 이동 키 입력 
            moveInput.x = Input.GetAxis("Horizontal");
            moveInput.y = Input.GetAxis("Vertical");
            moveInput.Normalize();

            // 효과 컨트롤 
            playerMovement.SetMoveInput(moveInput);
            playerAnimator.SetDirection(moveInput.x);

            // 점프 키 입력 / 슬라임 초록, 빨강 일때만 
            if (currentPlayerType == PlayerType.Default || currentPlayerType == PlayerType.Red)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    playerMovement.Jump();
                    playerAnimator.JumpAnim();
                }
            }
            #endregion

            #region 슬라임 타입 변경 / 머리 위 오브젝트 존재 x시에만
            if (CanStandup())
            {
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    ChangeSlimeType(PlayerType.Default, "Default");
                    changeEffect.PlayEffect();
                    playerAudio.PlayTFClip();
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    ChangeSlimeType(PlayerType.Red, "Red");
                    changeEffect.PlayEffect();
                    playerAudio.PlayTFClip();
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    ChangeSlimeType(PlayerType.Yellow, "Yellow");
                    changeEffect.PlayEffect();
                    playerAudio.PlayTFClip();
                }
                else if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    ChangeSlimeType(PlayerType.Blue, "Blue");
                    changeEffect.PlayEffect();
                    playerAudio.PlayTFClip();
                }
            }
            #endregion

            #region 맵 스캔 
            if (Input.GetKeyDown(KeyCode.R))
            {
                followPlayer.RetryMapScan();
            }
            #endregion
        }
        #endregion
    }

    #region 능력 비활성화 / 슬라임 전환 
    private void DisableAllAbilities()
    {
        redAbility.enabled = false;
        yellowAbility.enabled = false;
        blueAbility.enabled = false;
    }

    private void ChangeSlimeType(PlayerType newType, string animTrigger)
    {
        // 회전상태 초기화(회전 상태 버그 방지)
        StartCoroutine(ResetRotation());

        // 타입 변경 
        currentPlayerType = newType;
        playerAnimator.SetSlimeType(animTrigger);
        DisableAllAbilities();

        switch (currentPlayerType)
        {
            case PlayerType.Default:
                break;

            case PlayerType.Red:
                redAbility.enabled = true;
                break;

            case PlayerType.Yellow:
                yellowAbility.enabled = true;
                break;

            case PlayerType.Blue:
                blueAbility.enabled = true;
                break;
        }
    }

    IEnumerator ResetRotation()
    {
        float duration = 0.2f;
        float time = 0f;
        Quaternion startRot = transform.rotation;
        Quaternion endRot = Quaternion.identity;

        while (time < duration)
        {
            time += Time.deltaTime;
            transform.rotation = Quaternion.Lerp(startRot, endRot, time / duration);
            yield return null;
        }

        transform.rotation = endRot;
    }
    #endregion

    #region 죽음
    public void Dead()
    {
        if (isDead) return;

        isDead = true;

        // 물리 영향 안받게 처리 / 재자리 고정
        playerMovement.UnenabledMovement();

        // 애니메이션
        playerAnimator.DeadAnim();
        StartCoroutine(AfterDeadAnim());

        // 오디오 
        playerAudio.PlayDeadClip();
    }

    IEnumerator AfterDeadAnim()
    {
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
        UIManager.Instance.VisibleGameOverPanel();
    }
    #endregion

    #region 머리 위 오브젝트 체크 
    // Physics2D.Raycast(시작 위치, 방향, 거리, 레이어 마스크)
    public bool CanStandup()
    {
        Collider2D hit = Physics2D.OverlapBox(CheckBoxTransform.position, checkSize, 0f, groundLayer);

        return hit == null;
    }
    #endregion

    #region 시각화
    // 체크 박스 
    void OnDrawGizmos()
    {
        if (CheckBox != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(CheckBoxTransform.position, checkSize);
        }
    }
    #endregion
}