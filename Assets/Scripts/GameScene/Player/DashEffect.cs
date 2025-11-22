using UnityEngine;
using System.Collections;

public class DashEffect : MonoBehaviour
{
    private Animator animator;
    private PlayerAnimator playerAnimator;
    //좌표 
    [SerializeField] private Vector2 effectOffset = new Vector2(-0.375f, -0.035f);

    void Awake()
    {
        animator = GetComponent<Animator>();
        playerAnimator = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAnimator>();
    }

    void Start()
    {
        this.gameObject.SetActive(false);
    }

    public void PlayEffect()
    {
        if (playerAnimator.spriteRenderer.flipX == false)
        {
            transform.localPosition = effectOffset;
        }
        else
        {
            transform.localPosition = new Vector2(-effectOffset.x, effectOffset.y);
        }

        this.gameObject.SetActive(true);
        StartCoroutine(StopEffect());
    }

    IEnumerator StopEffect()
    {
        yield return new WaitForSeconds(0.4f);
        this.gameObject.SetActive(false);
    }
}
