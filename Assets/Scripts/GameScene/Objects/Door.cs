using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [SerializeField] private GameObject closedDoor;
    [SerializeField] private GameObject openedDoor;
    [SerializeField] private string TargetScene;

    void Awake()
    {
        closedDoor.SetActive(false);
        openedDoor.SetActive(false);
    }

    void Start()
    {
        closedDoor.SetActive(true);
    }

    // 충돌 이벤트 
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            closedDoor.SetActive(false);
            openedDoor.SetActive(true);
            SceneManager.LoadScene(TargetScene);
        }
    }
}
