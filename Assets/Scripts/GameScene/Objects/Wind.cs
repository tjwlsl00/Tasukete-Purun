using UnityEngine;
using System.Collections;

public class Wind : MonoBehaviour
{
    [SerializeField] float upwardForce = 10f;
    [SerializeField] GameObject WindEffect;

    void Awake()
    {
        WindEffect.gameObject.SetActive(false);
    }

    void Start()
    {
        WindVisible();
    }

    void WindVisible()
    {
        WindEffect.gameObject.SetActive(true);
        StartCoroutine(BlinkWind());
    }

    IEnumerator BlinkWind()
    {
        yield return new WaitForSeconds(0.5f);
        WindEffect.gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        WindVisible();
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("트리거 영역에 누군가 있음: " + collision.name);
            Rigidbody2D rigidbody2D = collision.GetComponent<Rigidbody2D>();
            if(rigidbody2D != null)
            {
                rigidbody2D.AddForce(Vector2.up * upwardForce, ForceMode2D.Force);
            }
        }
    }
}
