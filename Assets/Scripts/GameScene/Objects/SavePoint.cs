using UnityEngine;
using UnityEngine.SceneManagement;

public class SavePoint : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 1. 접촉된 오브젝트 위치 가져오기
            Vector2 playerPosition = collision.transform.position;
            
            // 2. 플레이어 위치 값 각각 저장
            PlayerPrefs.SetFloat("PlayerPosX", playerPosition.x);
            PlayerPrefs.SetFloat("PlayerPosY", playerPosition.y);

            // 3. 세이브된 씬 이름 저장 
            string currentSceneName = SceneManager.GetActiveScene().name;
            PlayerPrefs.SetString("SavedSceneName", currentSceneName);

            // 4. "저장돤 위치가 있다"는 정보 플래그로 저장 
            PlayerPrefs.SetInt("HasSavedPosition", 1);
            PlayerPrefs.Save();

            // 5.저장 이후에 사라져서 복수 세이브 방지 
            Debug.Log("플레이어 위치 저장" + playerPosition);
            gameObject.SetActive(false);
        }
    }
}
