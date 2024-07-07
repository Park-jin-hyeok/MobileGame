using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BottomBarManager : MonoBehaviour
{
    public static BottomBarManager Instance;

    public Button MainButton;
    public Button CharacterButton;
    public Button EquipmentButton;
    public Button GuildButton;
    public Button ShopButton;

    void Start()
    {
        MainButton.onClick.AddListener(() => LoadScene("MainScene"));
        CharacterButton.onClick.AddListener(() => LoadScene("CharacterScene"));
        EquipmentButton.onClick.AddListener(() => LoadScene("EquipmentScene"));
        GuildButton.onClick.AddListener(() => LoadScene("GuildScene"));
        ShopButton.onClick.AddListener(() => LoadScene("ShopScene"));
    }

    void LoadScene(string sceneName)
    {
        if (SceneManager.GetActiveScene().name != sceneName)
        {
            SceneManager.LoadScene(sceneName);
        }
    }
}
