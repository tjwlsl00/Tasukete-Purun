using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class Friend : MonoBehaviour
{
    [SerializeField] GameObject helpPanel;
    [SerializeField] GameObject TyPanel;
    private AudioSource audioSource;
    [SerializeField] AudioClip arigato;
    // bool
    private bool isContacted = false;

    void Awake()
    {
        helpPanel.SetActive(false);
        TyPanel.SetActive(false);

        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        VisibleHelp();
    }

    void VisibleHelp()
    {
        helpPanel.SetActive(true);
        StartCoroutine(BlinkHelpPanel());
    }

    IEnumerator BlinkHelpPanel()
    {
        yield return new WaitForSeconds(1f);
        helpPanel.SetActive(false);
        yield return new WaitForSeconds(1f);
        VisibleHelp();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 이중 충돌 방지
        if (isContacted) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            isContacted = true;
            Destroy(helpPanel);

            // 도와줘 깜빡임 효과 중지 / 고마워 텍스트 표시 
            StopCoroutine(BlinkHelpPanel());
            StartCoroutine(TyForHelp());

            // 각 매니저 수치, UI 업데이트
            GameManager.Instance.SaveSlimeCount++;
            UIManager.Instance.UpdateSaveSlimeUI(GameManager.Instance.SaveSlimeCount);

            if(GameManager.Instance.SaveSlimeCount == GameManager.Instance.maxSaveSlimeCount)
            {
                GameManager.Instance.TriggerVictroy();
            }
        }
    }

    IEnumerator TyForHelp()
    {
        yield return null;
        TyPanel.SetActive(true);
        audioSource.clip = arigato;
        audioSource.Play();
        yield return new WaitForSeconds(arigato.length);
        TyPanel.SetActive(false);
        Destroy(gameObject);
    }
}
