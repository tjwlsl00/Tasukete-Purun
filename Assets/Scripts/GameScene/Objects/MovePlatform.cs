using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    // 컴포넌트
    private Rigidbody2D rigidbody2D;
    // Transform
    public Transform PosA;
    public Transform PosB;
    private Vector2 currentTarget;
    // 오브젝트 수치
    [SerializeField] private float moveSpeed;

    void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // 처음 A위치로 이동 
        currentTarget = PosA.position;
    }

    void FixedUpdate()
    {
        Vector2 newPos = Vector2.MoveTowards(rigidbody2D.position, currentTarget, moveSpeed * Time.fixedDeltaTime);
        rigidbody2D.MovePosition(newPos);

        // 타겟과의 거리에 따라서 타겟 재설정 b -> a  / a -> b
        if (Vector2.Distance(rigidbody2D.position, currentTarget) < 0.01f)
        {
            if (currentTarget == (Vector2)PosA.position)
            {
                currentTarget = PosB.position;
            }
            else
            {
                currentTarget = PosA.position;
            }
        }
    }
}
