using UnityEngine;

public class PrisonDoorManager : MonoBehaviour
{
    private GameObject[] prisondoors;
    // 싱글톤
    public static PrisonDoorManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region 문 등록 / prisonDoorRegistry로 부터 오브젝트 전달 받음
    public void RegisterDoors(GameObject[] door)
    {
        this.prisondoors = door;
        ApplyClearedState();
    }
    #endregion

    #region 클리어 한 스테이지 문 비활성화
    private void ApplyClearedState()
    {
        if (prisondoors == null) return;

        for (int i =0; i < prisondoors.Length; i++)
        {
            if (i < GameManager.Instance.SaveSlimeCount)
            {
                prisondoors[i].SetActive(false);
            }
            else
            {
                prisondoors[i].SetActive(true);
            }
        }
    }
    #endregion
}
