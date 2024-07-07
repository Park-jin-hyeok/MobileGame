using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using TMPro;

public class GameInitializer : MonoBehaviour
{
    public string versionCheckUrl = "https://yourserver.com/checkversion";
    public string currentVersion = "1.0.0";
    public TextMeshProUGUI statusText;

    void Start()
    {
        StartCoroutine(CheckVersion());
    }

    IEnumerator CheckVersion()
    {
        statusText.text = "Checking version...";
        UnityWebRequest request = UnityWebRequest.Get(versionCheckUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            statusText.text = "Version check failed!";
        }
        else
        {
            string serverVersion = request.downloadHandler.text;

            if (serverVersion == currentVersion)
            {
                statusText.text = "Version is up-to-date.";
                LoadNextScene("MainGameScene");
            }
            else
            {
                statusText.text = "New version available. Redirecting to download resources...";
                LoadNextScene("LoadingScene");
            }
        }
    }

    void LoadNextScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
