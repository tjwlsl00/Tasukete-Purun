using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    // 컴포넌트 
    private AudioSource audioSource;
    // 오디오 파일 
    [SerializeField] AudioClip WalkClip;
    [SerializeField] AudioClip JumpClip;
    [SerializeField] AudioClip TFClip;
    [SerializeField] AudioClip DeadClip;
    [SerializeField] AudioClip StunClip;
    [SerializeField] AudioClip DashClip;
    [SerializeField] AudioClip WallBreakClip;

    void Awake()
    {
        // 컴포넌트 
        audioSource = GetComponent<AudioSource>();
    }

    #region 걷기 / 중단
    public void PlayWalkClip()
    {
        if (audioSource.isPlaying && audioSource.clip == WalkClip)
        {
            return;
        }

        audioSource.clip = WalkClip;
        audioSource.loop = true;
        audioSource.Play();
    }

    public void StopWalkClip()
    {
        if (audioSource.clip == WalkClip && audioSource.loop)
        {
            audioSource.loop = false;
            audioSource.Stop();
        }
    }
    #endregion

    // 점프
    public void PlayJumpClip()
    {
        audioSource.PlayOneShot(JumpClip);
    }

    // 변신
    public void PlayTFClip()
    {
        audioSource.PlayOneShot(TFClip);
    }

    // 대쉬 
    public void PlayDashClip()
    {
        audioSource.PlayOneShot(DashClip);
    }

    // 벽 부셔짐
    public void PlayWallBreakClip()
    {
        audioSource.PlayOneShot(WallBreakClip);
    }

    // 스턴
    public void PlayStunClip()
    {
        audioSource.PlayOneShot(StunClip);
    }

    // 죽음
    public void PlayDeadClip()
    {
        AudioSource.PlayClipAtPoint(DeadClip, transform.position);
    }

}