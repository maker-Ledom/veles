using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerData", menuName = "Player/Data")]
public class PlayerData : ScriptableObject
{
    [Header("Base Attributes")]
    public int baseStrength = 10;
    public int baseAgility = 10;

    [Header("Upgrade Costs")]
    public int baseUpgradeCost = 10;
    public float costMultiplier = 1.5f ;

    [Header("Scaling Coefficients")]
    public float hpPerStrength = 10f;
    public float dashCooldownReduction = 0.1f;
    
    [Header("Movement & Dash")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 20f;
    public float dashDistance = 5f;
    public float dashDuration = 0.2f;
    public float baseDashCooldown = 1.5f;
    public WeaponData startingWeapon;
}