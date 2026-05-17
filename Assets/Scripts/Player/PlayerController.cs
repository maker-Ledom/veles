using UnityEngine;
using System.Collections;
public class PlayerController : MonoBehaviour
{
    private PlayerModel _model;
    private PlayerData _data;
    private CharacterController _characterController;
    private bool _isDashing = false;
    private float _lastDashTime;
    public bool CanDash => !_isDashing && Time.time >= _lastDashTime + _model.CurrentDashCooldown;
    [Header("Weapon System")]
    [SerializeField] private Transform firePoint; // Точка, откуда вылетает пуля (конец ствола)
    public WeaponData currentWeapon;
    private float _nextFireTime;
    public void Init(PlayerModel model, PlayerData data)
    {
        _model = model;
        _data = data;
        _characterController = GetComponent<CharacterController>();
        model.OnWeaponChanged += OnWeaponChanged;
        currentWeapon = model.CurrentWeapon;
    }
    private void OnWeaponChanged(WeaponData newWeapon)
    {
        currentWeapon = newWeapon;
    }
    public void HandleLookRotation(Vector3 lookTarget)
    {
        Vector3 direction = lookTarget - transform.position;
        direction.y = 0; 

        if (direction.sqrMagnitude > 0.001f)
        {
            // Мгновенно вычисляем нужный поворот
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            
            // Назначаем его напрямую без Slerp
            transform.rotation = targetRotation;
        }
    }
    public IEnumerator Dash(Vector3 dashDirection)
    {
        _isDashing = true;
        _lastDashTime = Time.time;

        Vector3 finalDirection = dashDirection.sqrMagnitude > 0.001f ? dashDirection : transform.forward;
        float startTime = Time.time;

        while (Time.time < startTime + _data.dashDuration)
        {
            _characterController.Move(finalDirection * (_data.dashDistance / _data.dashDuration) * Time.deltaTime);
            yield return null;
        }

        _isDashing = false;
    }
    public void HandleMovement(Vector3 inputDirection)
    {
        if (_isDashing) return;
        if (inputDirection.magnitude >= 0.1f)
        {
            // Только перемещение. Вращение здесь БОЛЬШЕ НЕ НУЖНО.
            _characterController.Move(inputDirection * _data.moveSpeed * Time.deltaTime);
        }
    }
    public void TryShoot()
    {
        if (currentWeapon == null) return;

        if (Time.time >= _nextFireTime)
        {
            Shoot();
            _nextFireTime = Time.time + currentWeapon.fireRate;
        }
    }
    private void Shoot()
    {
        // Создаем пулю в точке выстрела
        GameObject bulletObj = Instantiate(currentWeapon.projectilePrefab, firePoint.position, transform.rotation);
        Projectile projectile = bulletObj.GetComponent<Projectile>();
        
        // Передаем параметры из текущего оружия
        projectile.Init(currentWeapon.damage, currentWeapon.projectileSpeed);
    }
    private void OnDestroy()
    {
        if (_model != null)
            _model.OnWeaponChanged -= OnWeaponChanged;
    }
}