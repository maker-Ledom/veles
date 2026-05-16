using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerData", menuName = "Player/Data")]
public class PlayerData : ScriptableObject
{
    [Header("Base Attributes")]
    public int baseStrength = 10;
    public int baseAgility = 10;

    [Header("Upgrade Costs")]
    public int baseUpgradeCost = 10;      // Начальная цена
    public float costMultiplier = 1.5f ;

    [Header("Scaling Coefficients")]
    public float hpPerStrength = 10f;       // 1 ед. силы = 10 HP
    public float dashCooldownReduction = 0.1f; // 1 ед. ловкости = -0.02 сек от кулдауна
    
    [Header("Movement & Dash")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 20f;
    public float dashDistance = 5f;
    public float dashDuration = 0.2f;
    public float baseDashCooldown = 1.5f; // Базовое время отката
    public WeaponData startingWeapon;
}