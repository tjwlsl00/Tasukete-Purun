using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator animator;
    public SpriteRenderer spriteRenderer;
    // 외부 스크립트 
    private PlayerMovement playerMovement;
    private RedAbility redAbility;

    // bool 
    private bool isFliped = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // 스크립트 
        playerMovement = GetComponent<PlayerMovement>();
        redAbility = GetComponent<RedAbility>();
    }

    void Update()
    {
        animator.SetBool("isWalking", playerMovement.isMoving);
    }

    #region 공통 / 이동시
    // 플레이어 뒤집기
    public void SetDirection(float horizontalInput)
    {
        // 레드상태 / 대쉬 /스턴 중에 플립 안하도록 
        if (redAbility.isDashing || redAbility.isStunned) return;

        // 그 외 상황 입력받은 키에 따라 캐릭터 방향전환 / Flip
        if (horizontalInput > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (horizontalInput < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

    // 슬라임 변신 
    public void SetSlimeType(string typeColor)
    {
        Debug.Log("### SetSlimeType 함수 호출됨! 인덱스: " + typeColor + " ###");
        animator.SetTrigger(typeColor);
    }
    #endregion

    #region 특정 상황
    // 1. 점프 (초록 / 빨강)
    public void JumpAnim()
    {
        animator.SetTrigger("isJumping");
    }

    // 2. 공통 / 죽음
    public void DeadAnim()
    {
        animator.SetTrigger("isDead");
    }
    #endregion

    #region 컷신 플레이어 뒤집기
    public void PlayerCutSceneFlip()
    {
        isFliped = !isFliped;

        if (isFliped)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }

    }
    #endregion

}