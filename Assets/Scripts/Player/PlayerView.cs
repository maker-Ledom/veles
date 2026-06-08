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
    [SerializeField] private GameObject statsPanel;
    [SerializeField] private TextMeshProUGUI strengthLevelText;
    [SerializeField] private TextMeshProUGUI agilityLevelText;
    [Header("Lifesaver Artifact")]
    [SerializeField] private Image lifesaverIcon;
    public void ToggleStatsPanel(bool isActive)
    {
        if (statsPanel != null) statsPanel.SetActive(isActive);
    }
    public void ShowLifesaverIcon(bool show)
    {
        if (lifesaverIcon != null)
            lifesaverIcon.gameObject.SetActive(show);
    }
    // PlayerView.cs – добавьте этот метод
public void RefreshLifesaverIcon(bool hasPerk)
{
    if (lifesaverIcon != null)
        lifesaverIcon.gameObject.SetActive(hasPerk);
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
            hpText.text = $"{current:f0} / {max:f0}";
        }
    }
    public void UpdateMoveAnimation(float forwardSpeed)
    {
        if (animator != null)
        {
            animator.SetFloat("Speed", forwardSpeed);
        }
    }
    public void UpdateCoins(int currentCoins)
    {
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