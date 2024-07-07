using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class ResourceDownloader : MonoBehaviour
{
    public string resourceUrl = "https://yourserver.com/resources";
    public Slider progressBar;
    public TextMeshProUGUI progressText;

    void Start()
    {
        StartCoroutine(DownloadResources());
    }

    IEnumerator DownloadResources()
    {
        UnityWebRequest request = UnityWebRequest.Get(resourceUrl);
        request.SendWebRequest();

        while (!request.isDone)
        {
            float progress = Mathf.Clamp01(request.downloadProgress / 0.9f);
            progressBar.value = progress;
            progressText.text = (progress * 100).ToString("F2") + "%";
            yield return null;
        }

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Resource download failed: " + request.error);
            progressText.text = "Download failed!";
        }
        else
        {
            Debug.Log("Resource download complete");
            progressBar.value = 1f;
            progressText.text = "100%";

            // 다운로드가 완료되면 다음 씬으로 전환
            LoadNextScene();
        }
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene("MainGameScene");
    }
}
