using UnityEngine;

public class YellowAbility : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float ClimbSpeed;
    private float targetRotation;
    private float originalGravity;
    private Rigidbody2D rigidbody2D;
    // bool 
    private bool isClimbing = false;

    void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        originalGravity = rigidbody2D.gravityScale;
        targetRotation = rigidbody2D.rotation;
    }

    // 비활성화 되면 벽에서 떨어짐
    void OnDisable()
    {
        StopClimbing(); 
    }

    void FixedUpdate()
    {
        // 등반 중에 키 입력 받아와서 이동시키기
        if (isClimbing)
        {
            rigidbody2D.gravityScale = 0;
            float horizontalInput = Input.GetAxis("Horizontal");

            Vector2 climbVelocity = transform.right * horizontalInput * ClimbSpeed;

            rigidbody2D.linearVelocity = climbVelocity;
        }

        // 회전
        /*
        <목표 각도와 현재 각도가 거의 같다면 계산을 멈춤>
        1. fixedDeltaTime : 물리 시계 / 천천히 규칙적으로 움직임 
        2. rigidbody2D.MoveRotation(newAngle); ->rotationSpeed를 이용해서 Rigidbody 회전 
        */
        if (Mathf.Abs(Mathf.DeltaAngle(rigidbody2D.rotation, targetRotation)) < 0.01f)
        {
            return;
        }
        // 현재 각도에서 목표 각도까지, rotationSpeed에 비례하는 만큼 살짝 이동한 이번 프레임의 새 각도를 계산해서 newAngle 변수에 저장
        else
        {
            float newAngle = Mathf.LerpAngle(rigidbody2D.rotation, targetRotation, Time.fixedDeltaTime * rotationSpeed);
            rigidbody2D.MoveRotation(newAngle);
        }
    }

    #region 충돌 이벤트 / 오브젝트 등반, 회전 
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (this.enabled && collision.gameObject.CompareTag("ClimbableWall"))
        {

            /*
           1. 빈 벡터(0, 0) 생성 
           2. foreach(벽에 닿는 모든 지점을 하나씩 확인) -> 벽이 밀어내는 방향 모두 더함
           3. .Normalize -> 평균 방향을 구한다 
           4. 평균 방향을 '각도' 숫자로 바꾼다.
           5. 플레이어가 벽과 평행하게 눕도록 -90도 
           */

            ContactPoint2D contact = collision.contacts[0];

            if (Mathf.Abs(contact.normal.y) < 0.5f)
            {
                isClimbing = true;
                rigidbody2D.gravityScale = 0;
                rigidbody2D.linearVelocity = Vector2.zero;

                CalculateAndSetRotation(collision);
            }

        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!this.enabled || !collision.gameObject.CompareTag("ClimbableWall")) return;

        if (!isClimbing)
        {
            ContactPoint2D contact = collision.contacts[0];

            if (Mathf.Abs(contact.normal.y) < 0.5f)
            {
                isClimbing = true;
                rigidbody2D.gravityScale = 0;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ClimbableWall"))
        {
            StopClimbing();
        }
    }

    // 회전 계산 로직
    void CalculateAndSetRotation(Collision2D collision)
    {
        Vector2 averageNormal = Vector2.zero;
        foreach (ContactPoint2D c in collision.contacts)
        {
            averageNormal += c.normal;
        }
        averageNormal.Normalize();
        float angle = Mathf.Atan2(averageNormal.y, averageNormal.x) * Mathf.Rad2Deg;
        targetRotation = angle - 90f;

    }
    #endregion

    // 등반 중지
    void StopClimbing()
    {
        isClimbing = false;
        rigidbody2D.gravityScale = originalGravity;

        // 플레이어를 평평하게 만들어준다. 회전 : 0도
        targetRotation = 0f;
    }
}