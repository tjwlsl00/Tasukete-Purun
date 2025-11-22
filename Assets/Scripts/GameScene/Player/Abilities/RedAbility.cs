using UnityEngine;
using System.Collections;

public class RedAbility : MonoBehaviour
{
    [SerializeField] private float DashSpeed;
    [SerializeField] private float DashDuration;
    [SerializeField] private float KnockbackForce;
    [SerializeField] private float StunDuration;
    private float originalGravity;
    // 컴포넌트 
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigidbody2D;
    // bool
    public bool isDashing;
    public bool isStunned;
    //효과 오브젝트
    [SerializeField] GameObject StunObject;
    [SerializeField] GameObject DashEffect;
    private DashEffect dashEffectEffect;
    // 스크립트 
    private PlayerMovement playerMovement;
    private PlayerAudio playerAudio;

    void Awake()
    {
        // 컴포넌트
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        originalGravity = rigidbody2D.gravityScale;

        // 스크립트 
        playerMovement = GetComponent<PlayerMovement>();
        playerAudio = GetComponent<PlayerAudio>();
        dashEffectEffect = DashEffect.GetComponent<DashEffect>();
    }

    void Start()
    {
        // 처음 스턴 오브젝트 안보이게 -> 대쉬 후 오브젝트가 파괴되었을때만 스턴 효과 
        StunObject.SetActive(false);
    }

    void Update()
    {
        if (!isDashing && !isStunned)
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                StartCoroutine(BreakDash());

                dashEffectEffect.PlayEffect();
                
                // 오디오 
                playerAudio.PlayDashClip();
            }
        }
    }

    // 대쉬 키 누르면 대쉬속도로 돌진 -> 도중에 부딪히는 오브젝트(부수기)
    IEnumerator BreakDash()
    {
        isDashing = true;

        // 오브젝트가 바라보는 방향
        Vector2 facingDirection = spriteRenderer.flipX ? Vector2.left : Vector2.right;

        // 중력 / 키 입력 무시 -> 대쉬 정상적으로 작동되게 
        playerMovement.canMove = false;
        rigidbody2D.gravityScale = 0;
        rigidbody2D.linearVelocity = facingDirection * DashSpeed;

        yield return new WaitForSeconds(DashDuration);

        // 벽에 부딪쳐 스턴 중이면 canMove false 상태 유지 
        if (isStunned)
        {
            isDashing = false;
            rigidbody2D.linearVelocity = Vector2.zero;
            rigidbody2D.gravityScale = originalGravity;
            yield break;
        }

        // 대쉬 시간 지나면 다시 정상으로 초기화
        isDashing = false;
        playerMovement.canMove = true;
        rigidbody2D.linearVelocity = Vector2.zero;
        rigidbody2D.gravityScale = originalGravity;
    }

    #region 충돌 이벤트 
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDashing) return;

        if (collision.gameObject.CompareTag("BreakWall"))
        {
            StartCoroutine(StunEffect());

            // 오디오 
            playerAudio.PlayWallBreakClip();

            Destroy(collision.gameObject);
        }
        else
        {
            rigidbody2D.linearVelocity = Vector2.zero;
            playerMovement.canMove = true;
            isDashing = false;
        }
    }
    #endregion

    #region 스턴 / 넉백 
    IEnumerator StunEffect()
    {
        isStunned = true;
        StunObject.SetActive(true);
        playerMovement.canMove = false;
        KnockBack();

        // 오디오 
        playerAudio.PlayStunClip();

        yield return new WaitForSeconds(StunDuration);
        isStunned = false;
        StunObject.SetActive(false);
        playerMovement.canMove = true;
    }

    private void KnockBack()
    {
        rigidbody2D.linearVelocity = Vector2.zero;

        // 넉백 방향 계산 
        Vector2 KnockbackDirectino = spriteRenderer.flipX ? Vector2.right : Vector2.left;

        // 해당 방향으로 넉백 
        rigidbody2D.AddForce(KnockbackDirectino * KnockbackForce, ForceMode2D.Impulse);
    }
    #endregion

}