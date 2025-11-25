using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerLoadPosition : MonoBehaviour
{
    void Awake()
    {
        if (PlayerPrefs.GetInt("HasSavedPosition") == 1)
        {
            string savedSceneName = PlayerPrefs.GetString("SavedSceneName");
            string currentName = SceneManager.GetActiveScene().name;

            if (savedSceneName == currentName)
            {
                float posX = PlayerPrefs.GetFloat("PlayerPosX");
                float posY = PlayerPrefs.GetFloat("PlayerPosY");

                Vector2 savedPosition = new Vector2(posX, posY);
                transform.position = savedPosition;
            }
        }
    }
}