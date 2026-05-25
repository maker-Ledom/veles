using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private Image activeWeaponIcon;
    [Header("Stats Panel")]
    [SerializeField] private GameObject statsPanel; // Сама панель (для вкл/выкл)
    [SerializeField] private TextMeshProUGUI strengthLevelText;
    [SerializeField] private TextMeshProUGUI agilityLevelText;
    public void ToggleStatsPanel(bool isActive)
    {
        if (statsPanel != null) statsPanel.SetActive(isActive);
    }

    public void UpdateStatsUI(int str, int agi)
    {
        if (strengthLevelText != null) 
            strengthLevelText.text = $"УРОВЕНЬ: {str}";
        
        if (agilityLevelText != null) 
            agilityLevelText.text = $"УРОВЕНЬ: {agi}";
    }
    public void SetupHealth(float maxHealth)
    {
        UpdateHealth(maxHealth, maxHealth);
    }
    public void UpdateHealth(float current, float max)
    {
        if (hpText != null)
        {
            // Форматируем строку. f0 убирает знаки после запятой (10.0 -> 10)
            hpText.text = $"{current:f0} / {max:f0}";
        }
    }

// В PlayerView.cs заменяем старый метод на этот:
    public void UpdateMoveAnimation(float forwardSpeed)
    {
        if (animator != null)
        {
            // Передаем значение в Float-параметр Speed вашего Blend Tree
            animator.SetFloat("Speed", forwardSpeed);
        }
    }
    public void UpdateCoins(int currentCoins)
    {
        // "N0" добавляет разделители тысяч для красоты (например, 1,000)
        coinsText.text = currentCoins.ToString();
    }
    public void UpdateWeaponIcon(WeaponData data)
    {
        if (activeWeaponIcon == null) return;
        if (data != null && data.icon != null)
        {
            activeWeaponIcon.sprite = data.icon;
            activeWeaponIcon.enabled = true;
        }
        else
        {
            activeWeaponIcon.enabled = false;
        }
    }
    public void PlayDeathAnimation()
    {
        if (animator != null)
        {
            // Срабатывает триггер смерти
            animator.SetTrigger("Die");
        }
    }
    public void PlayDashAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Dash");
        }
    }
}