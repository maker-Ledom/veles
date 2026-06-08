using System;
using UnityEngine;
using System.Collections.Generic;
public class PlayerModel
{
    public event Action<bool> OnLifesaverPerkChanged;
    public event Action OnDeath;
    public bool HasLifesaverPerk;
    private bool _isLifesaverUsed;
    public event Action OnLifesaverAcquired;
    public event Action OnLifesaverUsed;   
    public int Strength;
    public int Agility;
    public int Coins { get; private set; }
    public event Action<int> OnCoinsChanged;
    public event Action<int, int> OnStatsChanged;
    public event Action<WeaponData> OnWeaponChanged;
    public event Action<float, float> OnHealthChanged;
    public WeaponData CurrentWeapon { get; private set; }
    public List<string> PurchasedWeaponNames { get; private set; } = new List<string>();
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
    public void AddPurchasedWeapon(WeaponData weapon)
    {
        if (weapon != null && !PurchasedWeaponNames.Contains(weapon.name))
        {
            PurchasedWeaponNames.Add(weapon.name);
        }
    }
    public void AcquireLifesaverPerk()
    {
        if (HasLifesaverPerk) return;
        HasLifesaverPerk = true;
        _isLifesaverUsed = false;
        OnLifesaverAcquired?.Invoke();
        Debug.Log("Получен артефакт Сердце Феникса!");
    }
    public void TakeDamage(float amount)
    {
        if (CurrentHealth <= 0) return;
        CurrentHealth -= amount;
        
        if (HasLifesaverPerk && !_isLifesaverUsed && CurrentHealth <= MaxHealth * 0.2f)
        {
            ExecuteLifesaver();
        }
        
        if (CurrentHealth < 0) CurrentHealth = 0;
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        if (CurrentHealth <= 0) Die();
    }

    private void ExecuteLifesaver()
    {
        _isLifesaverUsed = true;
        HasLifesaverPerk = false;
        float healAmount = MaxHealth * 0.5f;
        CurrentHealth += healAmount;
        if (CurrentHealth > MaxHealth) CurrentHealth = MaxHealth;
        Debug.Log("<color=green>Сердце Феникса сработало! Здоровье восстановлено на 50%.</color>");
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        OnLifesaverUsed?.Invoke();
    }


    public void ResetPerksState()
    {
        HasLifesaverPerk = false;
        _isLifesaverUsed = false;
        OnLifesaverUsed?.Invoke();
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
        OnDeath?.Invoke();
    }
    public void UpgradeStrength(int amount)
    {
        Strength += amount;
        CurrentHealth += amount * _data.hpPerStrength; 
        OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
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
        return Mathf.RoundToInt(_data.baseUpgradeCost * Mathf.Pow(_data.costMultiplier, currentLevel));
    }
}