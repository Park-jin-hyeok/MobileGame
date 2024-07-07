using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
    public float introDuration = 3.0f; // ��Ʈ�� ȭ�� ǥ�� �ð�

    void Start()
    {
        // ��Ʈ�ΰ� ���� �� ���� ������ ��ȯ
        Invoke("LoadNextScene", introDuration);
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene("VersionCheckScene"); // ���� Ȯ�� �� �ε�
    }
}
