using UnityEngine;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour
{
    [SerializeField] Button menuButton;

    void Start()
    {
        menuButton = GetComponent<Button>();
        menuButton.onClick.AddListener(() => GameSceneManager.Instance.GoBackMenuScene());
    }
}
