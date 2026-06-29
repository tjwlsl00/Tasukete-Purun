using UnityEngine;

public class HeartMovement : MonoBehaviour
{
    [SerializeField] float moveDistance = 2f;
    [SerializeField] float moveSpeed = 5f; 

    private RectTransform rectTransform;
    private Vector2 StartPos;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        StartPos = rectTransform.anchoredPosition;
    }

    void Update()
    {
        float newY = StartPos.y + (Mathf.Sin(Time.time * moveSpeed) * moveDistance);
        rectTransform.anchoredPosition = new Vector2(StartPos.x, newY);
    }

}