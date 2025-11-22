using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuUIManager : MonoBehaviour
{
    [SerializeField] private Image ArrowRight;
    [SerializeField] private Image ArrowLeft;

    void Awake()
    {
        ArrowRight.gameObject.SetActive(false);
        ArrowLeft.gameObject.SetActive(false);
    }

    void Start()
    {
        ArrowVisible();
    }

    void ArrowVisible()
    {
        ArrowRight.gameObject.SetActive(true);
        ArrowLeft.gameObject.SetActive(true);
        StartCoroutine(BlinkArrow());
    }

    IEnumerator BlinkArrow()
    {
        yield return new WaitForSeconds(1f);
        ArrowRight.gameObject.SetActive(false);
        ArrowLeft.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        ArrowVisible();
    }
}
