using UnityEngine;
using System.Collections;

public class BreakScaffold : MonoBehaviour
{
    [SerializeField] float BreakTime = 2f;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Break());
        }
    }

    IEnumerator Break()
    {
        yield return new WaitForSeconds(BreakTime);
        Destroy(gameObject);
    }
}
