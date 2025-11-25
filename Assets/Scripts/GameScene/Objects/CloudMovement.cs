using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float StartPos;
    [SerializeField] float EndPos;
    private int moveDirection = -1;

    void Start()
    {
        StartPos = transform.position.x;

        if (EndPos < StartPos)
        {
            moveDirection = -1;
        }
        else
        {
            moveDirection = 1;
        }
    }

    void Update()
    {
        float currentX = transform.position.x;
        float nextX = currentX + (moveSpeed * moveDirection * Time.deltaTime);

        SetXPosition(nextX);
        CheckReset(nextX);
    }

    private void SetXPosition(float xPos)
    {
        transform.position = new Vector2(xPos, transform.position.y);
    }

    private void CheckReset(float currentXPos)
    {
        if (moveDirection == -1)
        {
            if (currentXPos <= EndPos)
            {
                SetXPosition(StartPos);
            }
        }
        else
        {
            if (currentXPos >= EndPos)
            {
                SetXPosition(StartPos);
            }
        }
    }
}
