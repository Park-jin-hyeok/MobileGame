using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    public float introDuration = 3.0f; // 인트로 화면 표시 시간

    void Start()
    {
        // 인트로가 끝난 후 다음 씬으로 전환
        Invoke("LoadNextScene", introDuration);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene("VersionCheckScene"); // 버전 확인 씬 로드
    }
}
