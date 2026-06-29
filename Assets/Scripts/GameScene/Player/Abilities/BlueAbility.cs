using UnityEngine;

public class BlueAbility : MonoBehaviour
{
    // 플레이어 크기 
    private Vector2 originalLocalScale;
    [SerializeField] Vector3 targetLocalScale;
    // bool
    private bool isSmall;
    // 외부 스크립트 
    private Player player;

    void Awake()
    {
        originalLocalScale = transform.localScale;
        // 외부 스크립트 
        player = GetComponent<Player>();
    }

    void Start()
    {
        isSmall = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            ToggleSize();
        }
    }

    #region 비활성화 될때
    void OnDisable()
    {
        transform.localScale = originalLocalScale;
        isSmall = false;
    }
    #endregion

    #region 사이즈 변경
    void ToggleSize()
    {
        isSmall = !isSmall;

        if (isSmall)
        {
            transform.localScale = targetLocalScale;
        }
        else
        {
            if (player.CanStandup())
            {
                isSmall = false;
                transform.localScale = originalLocalScale;
            }
            else
            {
                isSmall = true;
            }
        }
    }
    #endregion
}