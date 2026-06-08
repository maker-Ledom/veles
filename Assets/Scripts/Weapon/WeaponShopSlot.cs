using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class WeaponShopSlot : MonoBehaviour
{
    [Header("Weapon Settings")]
    public WeaponData weaponData;
    public int price;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private Button actionButton;
    [SerializeField] private TextMeshProUGUI buttonText;

    private PlayerModel playerModel;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            playerModel = GameManager.Instance.PlayerModel;
            playerModel.OnWeaponChanged += HandleWeaponChanged;
        }
        RefreshUI();
    }

    private void OnDisable()
    {
        if (playerModel != null)
        {
            playerModel.OnWeaponChanged -= HandleWeaponChanged;
        }
    }

    private void HandleWeaponChanged(WeaponData newWeapon)
    {
        if (this == null) return;
        RefreshUI();
    }

    public void OnClick()
    {
        if (playerModel == null || weaponData == null) return;

        bool isPurchased = playerModel.IsWeaponPurchased(weaponData);

        if (!isPurchased)
        {
            if (playerModel.SpendCoins(price))
            {
                playerModel.AddPurchasedWeapon(weaponData);
                playerModel.SetWeapon(weaponData);
            }
        }
        else
        {
            if (playerModel.CurrentWeapon != weaponData)
            {
                playerModel.SetWeapon(weaponData);
            }
        }
    }

    private void RefreshUI()
    {
        if (playerModel == null || weaponData == null) return;

        bool isCurrent = playerModel.CurrentWeapon == weaponData;
        bool isPurchased = playerModel.IsWeaponPurchased(weaponData);

        if (isCurrent)
        {
            priceText.gameObject.SetActive(false);
            buttonText.text = "ЭКИПИРОВАННО";
            actionButton.interactable = false;
        }
        else if (isPurchased)
        {
            priceText.gameObject.SetActive(false);
            buttonText.text = "ЭКИПИРОВАТЬ";
            actionButton.interactable = true;
        }
        else
        {
            priceText.gameObject.SetActive(true);
            priceText.text = price.ToString() + " МОНЕТ";
            buttonText.text = "КУПИТЬ";
            actionButton.interactable = true;
        }
    }
}