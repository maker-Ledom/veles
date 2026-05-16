using System;
using UnityEngine;
using System.Collections.Generic;
public class PlayerModel
{
    public event Action OnDeath;
    // ПЕРКИ (Артефакты)
    public bool HasLifesaverPerk;   // Есть ли у нас этот артефакт
    private bool _isLifesaverUsed; // Сработал ли он уже в этой жизни
    // Атрибуты
    public int Strength;
    public int Agility;
    public int Coins { get; private set; }
    
    // События для уведомления UI
    public event Action<int> OnCoinsChanged;
    public event Action<int, int> OnStatsChanged;
    public event Action<WeaponData> OnWeaponChanged;
    // НОВОЕ: Событие для изменения здоровья (передаем текущее и максимальное)
    public event Action<float, float> OnHealthChanged;
    
    public WeaponData CurrentWeapon { get; private set; }
    public List<string> PurchasedWeaponNames { get; private set; } = new List<string>();    // Состояние
    public float CurrentHealth;
    public float MaxHealth => Strength * _data.hpPerStrength;
    
    public float CurrentDashCooldown => Mathf.Max(0.2f, _data.baseDashCooldown - (Agility * _data.dashCooldownReduction));

    private PlayerData _data;

    public PlayerModel(PlayerData data)
    {
        _data = data;
        Strength = data.baseStrength;
        Agility = data.baseAgility;
        Coins = 0;
        CurrentHealth = data.baseStrength * data.hpPerStrength; 
        if (data.startingWeapon != null)
        {
            PurchasedWeaponNames.Add(data.startingWeapon.name);
            SetWeapon(data.startingWeapon);
        }
    }
    public bool IsWeaponPurchased(WeaponData weapon)
    {
        if (weapon == null) return false;
        return PurchasedWeaponNames.Contains(weapon.name);
    }

    // НОВОЕ: Метод добавления в инвентарь
    public void AddPurchasedWeapon(WeaponData weapon)
    {
        if (weapon != null && !PurchasedWeaponNames.Contains(weapon.name))
        {
            PurchasedWeaponNames.Add(weapon.name);
        }
    }

    public void TakeDamage(float amount)
    {
        if (CurrentHealth <= 0) return; // Чтобы не умирать дважды

        CurrentHealth -= amount;
        if (HasLifesaverPerk && !_isLifesaverUsed && CurrentHealth > 0 && CurrentHealth <= MaxHealth * 0.2f)
        {
            ExecuteLifesaver();
        }
        if (CurrentHealth < 0) CurrentHealth = 0;

        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }
    private void ExecuteLifesaver()
    {
        _isLifesaverUsed = true;
        
        // Восстанавливаем 50% от МАКСИМАЛЬНОГО здоровья
        float healAmount = MaxHealth * 0.5f;
        CurrentHealth += healAmount;
        
        // Не даем здоровью превысить максимум
        if (CurrentHealth > MaxHealth) CurrentHealth = MaxHealth;

        Debug.Log("<color=green>Сработал артефакт: Здоровье восстановлено на 50%!</color>");
        
        // Обязательно вызываем событие, чтобы UI обновился (цифры HP)
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
    }

    // Метод для сброса перков при смерти (вызывается из GameManager)
    public void ResetPerksState()
    {
        _isLifesaverUsed = false;
    }
    public void AddCoins(int amount)
    {
        if (amount < 0) return;
        Coins += amount;
        OnCoinsChanged?.Invoke(Coins);
    }

    public bool SpendCoins(int amount)
    {
        if (Coins >= amount)
        {
            Coins -= amount;
            OnCoinsChanged?.Invoke(Coins);
            return true;
        }
        return false;
    }

    public void SetWeapon(WeaponData newWeapon)
    {
        CurrentWeapon = newWeapon;
        OnWeaponChanged?.Invoke(newWeapon);
        if (!PurchasedWeaponNames.Contains(newWeapon.name))
    {
        PurchasedWeaponNames.Add(newWeapon.name);
    }

    OnWeaponChanged?.Invoke(newWeapon);

    }

    private void Die()
    {
        Debug.Log("Модель: Игрок погиб!");
        OnDeath?.Invoke(); // Оповещаем всех, кто слушает (например, GameManager)
    }
    public void UpgradeStrength(int amount)
    {
        Strength += amount;
        CurrentHealth += amount * _data.hpPerStrength; 
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        
        // Оповещаем об изменении статов
        OnStatsChanged?.Invoke(Strength, Agility);
    }

    public void UpgradeAgility(int amount)
    {
        Agility += amount;
        OnStatsChanged?.Invoke(Strength, Agility);
        
        Debug.Log($"Ловкость улучшена! Текущий уровень: {Agility}");
    }
    public int GetUpgradeCost(string statName)
    {
        int currentLevel = 0;
        
        if (statName == "Strength") currentLevel = Strength - _data.baseStrength;
        else if (statName == "Agility") currentLevel = Agility - _data.baseAgility;

        // Формула: Базовая цена * (Множитель в степени количества купленных уровней)
        // Например: 10 * (1.5 ^ 0) = 10; 10 * (1.5 ^ 1) = 15; 10 * (1.5 ^ 2) = 22...
        return Mathf.RoundToInt(_data.baseUpgradeCost * Mathf.Pow(_data.costMultiplier, currentLevel));
    }
    
}