using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public double gold;
    public double goldPerSecond;

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

    void Start()
    {
        SaveManager.Instance.LoadGame();
        StartCoroutine(GoldPerSecondCoroutine());
    }

    void OnApplicationQuit()
    {
        SaveManager.Instance.SaveGame();
    }

    IEnumerator GoldPerSecondCoroutine()
    {
        while (true)
        {
            AddGold(goldPerSecond);
            yield return new WaitForSeconds(1);
        }
    }

    public void AddGold(double amount)
    {
        gold += amount;
        UIManager.Instance.UpdateGoldText(gold);
    }
}
