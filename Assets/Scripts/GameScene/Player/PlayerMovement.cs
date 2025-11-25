using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Vector2 CurrentInput;
    private Rigidbody2D rigidbody2D;
    // 플레이어 변수
    [SerializeField] private float DefaultMoveSpeed;
    [SerializeField] private float DefaultJumpForce;
    [SerializeField] private float RedMoveSpeed;
    [SerializeField] private float RedJumpForce;
    [SerializeField] private float YAndBMoveSpeed;
    [SerializeField] private float WaterMoveSpeed;
    private float originalGravity;
    // bool
    public bool isGrounded;
    public bool canMove;
    public bool isMoving;
    public bool isJumping;
    public bool inWater;
    // 스크립트 
    private Player player;
    private PlayerAudio playerAudio;
    private RedAbility redAbility;
    // 플랫폼 이동
    private Rigidbody2D currentPlatformRb;
    private Vector2 lastPlatformPosition;

    void Awake()
    {
        // 컴포넌트
        rigidbody2D = GetComponent<Rigidbody2D>();
        // 스크립트
        player = GetComponent<Player>();
        playerAudio = GetComponent<PlayerAudio>();
        redAbility = GetComponent<RedAbility>();
    }

    void Start()
    {
        // 상태 초기화
        canMove = true;
        originalGravity = rigidbody2D.gravityScale;
    }

    public void SetMoveInput(Vector2 input)
    {
        if (!canMove)
        {
            CurrentInput = Vector2.zero;
            isMoving = false;

            if (playerAudio != null)
            {
                playerAudio.StopWalkClip();
            }
            return;
        }

        CurrentInput = input;
        isMoving = CurrentInput.sqrMagnitude > 0;

        if (playerAudio != null)
        {
            if (!redAbility.isDashing && isMoving)
            {
                playerAudio.PlayWalkClip();
            }
            else
            {
                playerAudio.StopWalkClip();
            }
        }
    }

    #region 물리 처리
    void FixedUpdate()
    {
        if (!canMove) return;

        // --------
        // 속도 
        // --------
        float xVelocity = 0f;
        if (player.currentPlayerType == Player.PlayerType.Default)
        {
            xVelocity = CurrentInput.x * DefaultMoveSpeed;
        }
        else if (player.currentPlayerType == Player.PlayerType.Red)
        {
            xVelocity = CurrentInput.x * RedMoveSpeed;
        }
        else if (player.currentPlayerType == Player.PlayerType.Yellow || player.currentPlayerType == Player.PlayerType.Blue)
        {
            xVelocity = CurrentInput.x * YAndBMoveSpeed;
        }

        #region 이동 
        // 기본 이동 
        if (currentPlatformRb == null)
        {
            rigidbody2D.linearVelocity = new Vector2(xVelocity, rigidbody2D.linearVelocity.y);
        }
        // 플랫폼 탑승 시 
        else
        {
            Vector2 deltaPosition = currentPlatformRb.position - lastPlatformPosition;
            Vector2 platformVelocity = deltaPosition / Time.fixedDeltaTime;
            lastPlatformPosition = currentPlatformRb.position;

            // X축 플레이어 속도 + 플랫폼 속도 
            float finalXVelocity = xVelocity + platformVelocity.x;
            // y축 플레이어 속도
            float finalYVelocity = rigidbody2D.linearVelocity.y;

            if (platformVelocity.y < 0)
            {
                // 플레이어 Y속도와 플랫폼 속도 중에 더 낮은 값으로 설정 
                finalYVelocity = Mathf.Min(rigidbody2D.linearVelocity.y, platformVelocity.y);
            }
            rigidbody2D.linearVelocity = new Vector2(finalXVelocity, finalYVelocity);
        }

        // 물 속 이동 
        if (inWater)
        {
            rigidbody2D.gravityScale = 0.2f;
            rigidbody2D.linearDamping = 3f;

            // 물 속으로 떨어질때 낙하 속도 조절
            if(rigidbody2D.linearVelocity.y < -2f)
            {
                rigidbody2D.linearVelocity = new Vector2(rigidbody2D.linearVelocity.x, -2f);
            }

            // 물 속에서 캐릭터 이동 입력
            if (Mathf.Abs(CurrentInput.x) > 0.1f || Mathf.Abs(CurrentInput.y) > 0.1f)
            {
                rigidbody2D.linearVelocity = new Vector2(CurrentInput.x * WaterMoveSpeed, CurrentInput.y * WaterMoveSpeed);
            }
            else
            {
                // Impulse: 순간적인 힘 / Force : 물리 우선 
                rigidbody2D.AddForce(Vector2.up * 6f, ForceMode2D.Force);
            }
        }
        else
        {
            rigidbody2D.gravityScale = originalGravity;
            rigidbody2D.linearDamping = 0f;
        }
        #endregion
    }
    #endregion

    #region 점프
    public void Jump()
    {
        if (!isGrounded) return;
        isJumping = true;

        if (player.currentPlayerType == Player.PlayerType.Default)
        {
            rigidbody2D.AddForce(Vector2.up * DefaultJumpForce, ForceMode2D.Impulse);
        }
        else if (player.currentPlayerType == Player.PlayerType.Red)
        {
            rigidbody2D.AddForce(Vector2.up * RedJumpForce, ForceMode2D.Impulse);
        }

        if (playerAudio != null)
        {
            playerAudio.PlayJumpClip();
        }
    }
    #endregion

    #region 물리 비활성화 / 활성화
    // 죽음 & 컷신시 멈추게 
    public void UnenabledMovement()
    {
        canMove = false;
        SetMoveInput(Vector2.zero);

        rigidbody2D.linearVelocity = Vector2.zero;
        rigidbody2D.bodyType = RigidbodyType2D.Kinematic;
    }
    // 다시 이동 
    public void enabledMovement()
    {
        canMove = true;
        rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
    }
    #endregion

    #region 충돌 이벤트 
    void OnCollisionEnter2D(Collision2D collision)
    {
        #region 땅 
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            if (isJumping)
            {
                isJumping = false;
            }
        }
        #endregion

        #region 무빙 플랫폼
        // 접촉한 오브젝트가 MovePlatform 스크립트 가지고 있는지 체크 
        MovePlatform movePlatform = collision.gameObject.GetComponent<MovePlatform>();
        if (movePlatform != null)
        {
            // 접촉 오브젝트 rigidbody2D 가져오기
            Rigidbody2D platformRb = collision.gameObject.GetComponent<Rigidbody2D>();
            if (platformRb != null)
            {
                // 현재 플랫폼으로 설정하고 위치 추적
                currentPlatformRb = platformRb;
                lastPlatformPosition = currentPlatformRb.position;
            }
        }
        #endregion
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        #region 땅
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
        #endregion

        #region 무빙 플랫폼 
        // 접촉한 오브젝트가 MovePlatform 스크립트 가지고 있는지 체크 
        MovePlatform movePlatform = collision.gameObject.GetComponent<MovePlatform>();
        if (movePlatform != null)
        {
            // 접촉 오브젝트 rigidbody2D 가져오기
            Rigidbody2D platformRb = collision.gameObject.GetComponent<Rigidbody2D>();

            // 점프하게 되면 플랫폼 null 처리 
            if (platformRb == currentPlatformRb)
            {
                if (isJumping)
                {
                    currentPlatformRb = null;
                }
            }
        }
        #endregion
    }

    // 물 안에 있을때 
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Water"))
        {
            inWater = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Water"))
        {
            inWater = false;
        }
    }
    #endregion

}