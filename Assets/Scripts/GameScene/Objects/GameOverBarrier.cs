using UnityEngine;

public class GameOverBarrier : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.TriggerGameOver();
        }
    }
}
    