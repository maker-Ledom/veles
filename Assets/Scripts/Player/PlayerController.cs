using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private PlayerModel _model;
    private PlayerData _data;
    private CharacterController _characterController;
    private bool _isDashing = false;
    private float _lastDashTime;
    private Vector3 _velocity;
    private bool _isGrounded;
    public bool CanDash => !_isDashing && Time.time >= _lastDashTime + _model.CurrentDashCooldown;

    [Header("Weapon System")]
    [SerializeField] private Transform firePoint;
    public WeaponData currentWeapon;
    private float _nextFireTime;

    [Header("Gravity Settings")]
    [SerializeField] private float gravityMultiplier = 1f;

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
            Quaternion targetRotation = Quaternion.LookRotation(direction);
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

        Vector3 move = inputDirection * _data.moveSpeed;
        if (_characterController.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        _velocity.y += Physics.gravity.y * gravityMultiplier * Time.deltaTime;
        move.y = _velocity.y;

        _characterController.Move(move * Time.deltaTime);

        _isGrounded = _characterController.isGrounded;
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
        GameObject bulletObj = Instantiate(currentWeapon.projectilePrefab, firePoint.position, transform.rotation);
        Projectile projectile = bulletObj.GetComponent<Projectile>();
        projectile.Init(currentWeapon.damage, currentWeapon.projectileSpeed);
    }

    private void OnDestroy()
    {
        if (_model != null)
            _model.OnWeaponChanged -= OnWeaponChanged;
    }
}