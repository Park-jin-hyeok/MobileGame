using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public double upgradeCost = 10;
    public double goldPerSecondIncrease = 1;

    public void BuyUpgrade()
    {
        if (GameManager.Instance.gold >= upgradeCost)
        {
            GameManager.Instance.gold -= upgradeCost;
            GameManager.Instance.goldPerSecond += goldPerSecondIncrease;
            UIManager.Instance.UpdateGoldText(GameManager.Instance.gold);
            UIManager.Instance.UpdateGoldPerSecondText(GameManager.Instance.goldPerSecond);
        }
    }
}
