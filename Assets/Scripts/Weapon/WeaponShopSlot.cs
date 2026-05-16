using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponShopSlot : MonoBehaviour
{
    [Header("Настройки оружия")]
    public WeaponData weaponData;
    public int price;

    [Header("UI Элементы")]
    [SerializeField] private TextMeshProUGUI priceText;    
    [SerializeField] private Button actionButton;         
    [SerializeField] private TextMeshProUGUI buttonText;  

    private PlayerModel _model;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            _model = GameManager.Instance.PlayerModel;
            _model.OnWeaponChanged += HandleWeaponChanged;
        }
        RefreshUI();
    }

    private void OnDisable() // Или OnDestroy, чтобы избежать ошибки со ссылками
    {
        if (_model != null)
            _model.OnWeaponChanged -= HandleWeaponChanged;
    }

    private void HandleWeaponChanged(WeaponData newData)
    {
        if (this == null) return;
        RefreshUI(); 
    }

    public void OnClick()
    {
        if (_model == null) return;

        // Спрашиваем ИНВЕНТАРЬ ИГРОКА, куплено ли оружие
        bool isPurchased = _model.IsWeaponPurchased(weaponData);

        if (!isPurchased)
        {
            // Покупка
            if (_model.SpendCoins(price))
            {
                _model.AddPurchasedWeapon(weaponData); // Кладем в рюкзак
                _model.SetWeapon(weaponData);          // Берем в руки
            }
        }
        else
        {
            // Смена экипировки
            if (_model.CurrentWeapon != weaponData)
            {
                _model.SetWeapon(weaponData);          // Берем в руки бесплатно
            }
        }
    }

    public void RefreshUI()
    {
        if (_model == null || weaponData == null) return;

        bool isCurrent = _model.CurrentWeapon == weaponData;
        bool isPurchased = _model.IsWeaponPurchased(weaponData); // Проверяем рюкзак!
        Debug.Log($"Слот магазина [{weaponData.name}]: В руках? {isCurrent} | Куплено? {isPurchased} | Всего пушек в рюкзаке: {_model.PurchasedWeaponNames.Count}");
        if (isCurrent)
        {
            priceText.gameObject.SetActive(false); 
            buttonText.text = "ИСПОЛЬЗУЕТСЯ";
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
            priceText.text = $"{price} МОНЕТ";
            buttonText.text = "КУПИТЬ";
            actionButton.interactable = true;
        }
    }
}