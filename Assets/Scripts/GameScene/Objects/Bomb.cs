using UnityEngine;
using System.Collections;

public class Bomb : MonoBehaviour
{
    private Animator animator;
    [SerializeField] AudioClip explosionClip;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Explosion());
        }
    }

    IEnumerator Explosion()
    {
        animator.SetTrigger("Explosion");
        yield return null;

        GameManager.Instance.TriggerGameOver();

        float animationDuration = animator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(animationDuration);
        
        // 사운드 및 오브젝트 삭제 
        AudioSource.PlayClipAtPoint(explosionClip, transform.position, 2f);
        Destroy(gameObject);
    }
}