using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI goldText;
    public TextMeshProUGUI goldPerSecondText;

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

    public void UpdateGoldText(double gold)
    {
        goldText.text = "Gold: " + gold.ToString("F2");
    }

    public void UpdateGoldPerSecondText(double goldPerSecond)
    {
        goldPerSecondText.text = "Gold per second: " + goldPerSecond.ToString("F2");
    }
}
