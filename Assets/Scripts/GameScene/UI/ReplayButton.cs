using UnityEngine;
using UnityEngine.UI;

public class ReplayButton : MonoBehaviour
{
    [SerializeField] Button replayButton;

    void Start()
    {
        replayButton = GetComponent<Button>();
        replayButton.onClick.AddListener(() => GameSceneManager.Instance.ReloadCurrentScene());
    }
}
