using UnityEngine;
using TMPro;

public class StatsUpgrader : MonoBehaviour
{
    public enum StatType { Strength, Agility }
    public StatType targetStat;
    
    public int upgradePrice = 50;
    public GameObject upgradePanel;
    [SerializeField] private TextMeshProUGUI priceText; // Ссылка на текст цены в UI магазина
    [SerializeField] private string statToUpgrade;

    private void Start()
    {
        if (priceText != null) priceText.text = upgradePrice.ToString();
    }
    public void RefreshUI()
    {
        if (GameManager.Instance != null)
        {
            int currentPrice = GameManager.Instance.PlayerModel.GetUpgradeCost(statToUpgrade);
            priceText.text = $"ЦЕНА: {currentPrice}";
        }
    }
    public void OpenPanel()
    {
        upgradePanel.SetActive(true);
        
        // Сообщаем менеджеру, что интерфейс занят
        if (GameManager.Instance != null)
        {
            GameManager.Instance.isAnyPanelOpen = true;
        }

        Time.timeScale = 0f; 
    }

    public void ClosePanel()
    {
        upgradePanel.SetActive(false);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.isAnyPanelOpen = false;
            GameManager.Instance.ResumeGameplayState();
        }
    }

    public void TryUpgrade()
    {
        var model = GameManager.Instance.PlayerModel;
        int price = model.GetUpgradeCost(statToUpgrade);

        if (model.SpendCoins(price))
        {
            if (statToUpgrade == "Strength") model.UpgradeStrength(1);
            else model.UpgradeAgility(1);
            
            // Сразу обновляем цену на кнопке после покупки
            RefreshUI();
        }
        else
        {
            Debug.Log("Недостаточно монет!");
        }
    }
}