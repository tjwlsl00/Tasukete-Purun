using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.UI;

public class CutSceneManager : MonoBehaviour
{
    public enum Speaker
    {
        Player,
        Enemy,
        Friend
    }

    [System.Serializable]
    public class CutSceneStep
    {
        public Speaker speaker;
        [TextArea] public string message;
        public float duration = 2f;
        public UnityEvent onStartEvent;
    }

    // 대화 오브젝트 
    [SerializeField] GameObject dialoguePanel;
    [SerializeField] GameObject playerBubble;
    [SerializeField] TextMeshProUGUI playerText;
    [SerializeField] GameObject enemyBubble;
    [SerializeField] TextMeshProUGUI enemyText;
    [SerializeField] GameObject FriendBubble;
    [SerializeField] TextMeshProUGUI FriendText;
    public List<CutSceneStep> scenarioList;
    // bool
    private bool isPlaying = false;
    // 페이드 인
    [SerializeField] GameObject FadeInPanel;
    [SerializeField] GameObject EndImage;
    [SerializeField] private TextMeshProUGUI EndingText;
    [SerializeField] float TypingSpeed = 1.0f;
    [TextArea(3, 5)]
    [SerializeField] string EndMessage;

    void Start()
    {
        FadeInPanel.SetActive(false);
    }

    #region 컷신 시작 / 끝 
    public void StartCutScene()
    {
        if (isPlaying) return;
        isPlaying = true;
        dialoguePanel.SetActive(true);
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        foreach (var step in scenarioList)
        {
            // 해당 순서 이벤트 존재하는 경우 실행 
            if (step.onStartEvent != null)
            {
                step.onStartEvent.Invoke();
            }

            if (!string.IsNullOrEmpty(step.message))
            {
                ShowBubble(step.speaker, step.message);
                yield return new WaitForSeconds(step.duration);
            }
        }

        EndCutScene();
    }

    void EndCutScene()
    {
        isPlaying = false;
        dialoguePanel.SetActive(false);

        // 페이드 인 
        FadeIn();
    }
    #endregion

    void ShowBubble(Speaker speaker, string msg)
    {
        // 모든 말풍선 끄기
        playerBubble.SetActive(false);
        enemyBubble.SetActive(false);
        FriendBubble.SetActive(false);

        // 해당 되는 사람만 켜기 
        if (speaker == Speaker.Player)
        {
            playerBubble.SetActive(true);
            playerText.text = msg;
        }
        else if (speaker == Speaker.Enemy)
        {
            enemyBubble.SetActive(true);
            enemyText.text = msg;
        }
        else if (speaker == Speaker.Friend)
        {
            FriendBubble.SetActive(true);
            FriendText.text = msg;
        }
    }

    #region 페이드 인 / 타자 코루틴 /엔딩 이미지
    void FadeIn()
    {
        FadeInPanel.SetActive(true);

        Image PanelBackground = FadeInPanel.GetComponent<Image>();

        if (PanelBackground != null)
        {
            PanelBackground.canvasRenderer.SetAlpha(0.0f);
            PanelBackground.CrossFadeAlpha(1.0f, 1f, false);
        }

        StartCoroutine(TypingCoroutine(EndMessage));
    }

    IEnumerator TypingCoroutine(string msg)
    {
        if (msg == null) yield break;

        EndingText.text = "";

        foreach (char letter in msg.ToCharArray())
        {
            EndingText.text += letter;
            yield return new WaitForSeconds(TypingSpeed);
        }

        EndImageFadeIn();
    }

    void EndImageFadeIn()
    {
        // Ending이미지 활성화
        EndImage.SetActive(true);

        Image image = EndImage.GetComponent<Image>();

        if (image != null)
        {
            image.canvasRenderer.SetAlpha(0.0f);
            image.CrossFadeAlpha(1.0f, 1f, false);
        }

        StartCoroutine(GoToMenuScene());
    }

    // 5초 경과 후 메뉴 씬으로 이동
    IEnumerator GoToMenuScene()
    {
        yield return new WaitForSeconds(5.0f);
        GameSceneManager.Instance.GoBackMenuScene();
    }
    #endregion

}
