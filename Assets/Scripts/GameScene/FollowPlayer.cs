using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] Vector2 minCameraBoundary;
    [SerializeField] Vector2 maxCameraBoundary;
    // 스캔 시간
    [SerializeField] float scanDuration = 3f;
    // 애니메이션 효과 부드럽게 / S자 커브로 설정 
    [SerializeField] AnimationCurve scanCurve;
    // 카메라 Z축 위치 
    private float cameraZ;
    // bool
    private bool isScanning = true;

    void Start()
    {
        cameraZ = transform.position.z;

        // 현재 활성화 된 씬 이름 가져오기 
        string currentName = SceneManager.GetActiveScene().name;

        if (currentName == "TutorialScene")
        {
            isScanning = false;
            transform.position = GetClampedPosition(Player.playerTransform.position);
        }
        else
        {
            // 게임 매니저에 카메라 스캔된 씬 저장 되었는지 
            if (GameManager.Instance.scannedStages.Contains(currentName) == false)
            {
                StartCoroutine(ScanMap(maxCameraBoundary, minCameraBoundary));
            }
            else
            {
                isScanning = false;
                transform.position = GetClampedPosition(Player.playerTransform.position);
            }
        }
    }

    // 맵 스캔 
    IEnumerator ScanMap(Vector3 startPos, Vector3 endPos)
    {
        string currentName = SceneManager.GetActiveScene().name;
        GameManager.Instance.scannedStages.Add(currentName);

        // 스캔 동안 플레이어 움직임 방지 
        GameManager.Instance.GameCurrentDireaction = GameManager.GameManagerDireaction.Scan;

        isScanning = true;
        float elapsedTime = 0f;

        startPos.z = cameraZ;
        endPos.z = cameraZ;

        transform.position = startPos;

        while (elapsedTime < scanDuration)
        {
            float t = elapsedTime / scanDuration;
            float curveValue = scanCurve.Evaluate(t);
            transform.position = Vector3.Lerp(startPos, endPos, curveValue);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        isScanning = false;

        // 스캔 끝나면 다시 움직임 가능하게 
        GameManager.Instance.GameCurrentDireaction = GameManager.GameManagerDireaction.Play;
    }

    #region 플레이어 따라가기 
    private void LateUpdate()
    {
        if (isScanning || Player.playerTransform == null) return;
        transform.position = GetClampedPosition(Player.playerTransform.position);
    }

    private Vector3 GetClampedPosition(Vector3 position)
    {
        float clampedX = Mathf.Clamp(position.x, minCameraBoundary.x, maxCameraBoundary.x);
        float clampedY = Mathf.Clamp(position.y, minCameraBoundary.y, maxCameraBoundary.y);

        return new Vector3(clampedX, clampedY, cameraZ);
    }
    #endregion

    #region 플레이어 임의 맵 스캔 
    public void RetryMapScan()
    {
        // 게임 매니저에서 현재 씬 정보 삭제 후 다시 등록 
        string currentName = SceneManager.GetActiveScene().name;

        if (currentName == "TutorialScene" || currentName == "FinalScene")
        {
            Debug.Log("현재 맵 스캔 불가");
            return;
        }
        else
        {
            GameManager.Instance.scannedStages.Remove(currentName);
            StartCoroutine(ScanMap(maxCameraBoundary, minCameraBoundary));
        }

    }
    #endregion
}
