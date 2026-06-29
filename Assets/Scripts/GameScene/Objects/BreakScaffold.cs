using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class BreakScaffold : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] float BreakTime = 2f;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Break());
        }
    }

    IEnumerator Break()
    {
        float currentTime = 0f;
        Color startColor = spriteRenderer.color;

        while(currentTime < BreakTime)
        {
            currentTime += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, currentTime / BreakTime);

            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }
        Destroy(gameObject);
    }
}
