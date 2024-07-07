using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

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

    public void SaveGame()
    {
        PlayerPrefs.SetString("Gold", GameManager.Instance.gold.ToString());
        PlayerPrefs.SetString("GoldPerSecond", GameManager.Instance.goldPerSecond.ToString());
    }

    public void LoadGame()
    {
        if (PlayerPrefs.HasKey("Gold"))
        {
            GameManager.Instance.gold = double.Parse(PlayerPrefs.GetString("Gold"));
            GameManager.Instance.goldPerSecond = double.Parse(PlayerPrefs.GetString("GoldPerSecond"));
            UIManager.Instance.UpdateGoldText(GameManager.Instance.gold);
            UIManager.Instance.UpdateGoldPerSecondText(GameManager.Instance.goldPerSecond);
        }
    }
}
