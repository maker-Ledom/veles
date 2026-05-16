using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Player/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public bool isPurchased;
    public string weaponName;
    public Sprite icon;
    public float damage = 10f;
    public float fireRate = 0.2f; // Пауза между выстрелами в секундах
    public float projectileSpeed = 20f;
    public GameObject projectilePrefab; // Префаб пули
}