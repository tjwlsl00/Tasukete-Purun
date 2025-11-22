using System.Collections;
using UnityEngine;

public class ChangeEffect : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        this.gameObject.SetActive(false);
    }

    public void PlayEffect()
    {
        this.gameObject.SetActive(true);
        StartCoroutine(StopEffect());
    }

    IEnumerator StopEffect()
    {
        yield return new WaitForSeconds(0.4f);
        this.gameObject.SetActive(false);
    }
}
