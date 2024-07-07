using UnityEngine;

public class ResourceCollector : MonoBehaviour
{
    public void CollectGold(double amount)
    {
        GameManager.Instance.AddGold(amount);
    }
}
