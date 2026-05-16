using UnityEngine;

public class ShopButton : MonoBehaviour
{
    public WeaponData weaponToBuy; // Здесь имя: weaponToBuy
    public int price;

    public void OnPurchaseClick()
    {
        PlayerRuntime player = FindFirstObjectByType<PlayerRuntime>();
        if (player == null || weaponToBuy == null) return;

        PlayerModel model = player.Model;

        // 1. СНАЧАЛА ПРОВЕРЯЕМ: Куплено ли уже это оружие?
        if (model.IsWeaponPurchased(weaponToBuy))
        {
            // Если куплено — просто экипируем бесплатно
            if (model.CurrentWeapon != weaponToBuy)
            {
                model.SetWeapon(weaponToBuy);
                Debug.Log($"Бесплатная экипировка: {weaponToBuy.name}");
            }
        }
        else
        {
            // 2. ЕСЛИ НЕ КУПЛЕНО — пытаемся списать деньги
            if (model.SpendCoins(price))
            {
                model.AddPurchasedWeapon(weaponToBuy);
                model.SetWeapon(weaponToBuy);
                Debug.Log($"Куплено впервые: {weaponToBuy.name}");
            }
            else
            {
                Debug.Log("Недостаточно монет для покупки!");
            }
        }
    }
}