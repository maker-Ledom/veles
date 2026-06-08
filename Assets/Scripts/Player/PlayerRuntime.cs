using UnityEngine;
public class PlayerRuntime : MonoBehaviour
{
    [SerializeField] private PlayerData data;
    [SerializeField] private PlayerView view;
    private PlayerModel model;
    private PlayerController controller;
    private Camera mainCamera;
    private bool isDead = false;

    private void Awake()
    {
        if (GameManager.Instance != null && GameManager.Instance.PlayerModel != null)
        {
            model = GameManager.Instance.PlayerModel;
        }
        else
        {
            model = new PlayerModel(data);
        }

        controller = GetComponent<PlayerController>();
        controller.Init(model, data);

        model.OnCoinsChanged += view.UpdateCoins;
        model.OnWeaponChanged += view.UpdateWeaponIcon;
        model.OnHealthChanged += view.UpdateHealth;
        model.OnStatsChanged += view.UpdateStatsUI;
        model.OnDeath += OnPlayerDeath;
        model.OnLifesaverAcquired += OnLifesaverAcquired;
        model.OnLifesaverUsed += OnLifesaverUsed;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        view.UpdateStatsUI(model.Strength, model.Agility);
        view.UpdateCoins(model.Coins);
        view.SetupHealth(model.MaxHealth);
        
        if (model.CurrentHealth <= 0)
            model.CurrentHealth = model.MaxHealth;
        view.UpdateHealth(model.CurrentHealth, model.MaxHealth);
        
        if (model.CurrentWeapon != null)
            view.UpdateWeaponIcon(model.CurrentWeapon);

        view.RefreshLifesaverIcon(model.HasLifesaverPerk);
    }
    
    private void Update()
    {
        if (isDead) return;
        
        if (GameManager.Instance != null && GameManager.Instance.IsGameBlocked())
            return;

        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        Vector2 input = new Vector2(moveX, moveZ);

        Vector3 camForward = mainCamera.transform.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = mainCamera.transform.right;
        camRight.y = 0;
        camRight.Normalize();

        Vector3 cameraRelativeDir = (camForward * input.y + camRight * input.x).normalized;

        RotatePlayerToMouse();

        if (Input.GetKeyDown(KeyCode.Space) && controller.CanDash)
        {
            StartCoroutine(controller.Dash(cameraRelativeDir));
            view.PlayDashAnimation(); // Запуск триггера "Dash"
        }
        else
        {
            controller.HandleMovement(cameraRelativeDir);
        }
        float forwardSpeed = 0f;
        if (cameraRelativeDir.sqrMagnitude > 0.01f)
        {
            forwardSpeed = Vector3.Dot(cameraRelativeDir, transform.forward);
        }

        view.UpdateMoveAnimation(forwardSpeed);

        if (Input.GetMouseButton(0))
        {
            controller.TryShoot();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            view.ToggleStatsPanel(true);
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            view.ToggleStatsPanel(false);
        }
    }
    private void RotatePlayerToMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        float targetHeight = transform.position.y + 1f;
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, targetHeight, 0));

        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 pointToLook = ray.GetPoint(rayDistance);
            controller.HandleLookRotation(pointToLook);
        }
    }

    public void ApplyDamage(float damage)
    {
        if (isDead) return;
        model.TakeDamage(damage);
    }

    public void CollectCoin(int amount)
    {
        model.AddCoins(amount);
    }

    private void OnPlayerDeath()
    {
        isDead = true;
        view.PlayDeathAnimation();
    }
    private void OnLifesaverAcquired()
    {
        view.RefreshLifesaverIcon(true);
    }

    private void OnLifesaverUsed()
    {
        view.RefreshLifesaverIcon(false);
    }
    private void OnDisable()
    {
        if (model != null)
        {
            model.OnCoinsChanged -= view.UpdateCoins;
            model.OnWeaponChanged -= view.UpdateWeaponIcon;
            model.OnHealthChanged -= view.UpdateHealth;
            model.OnStatsChanged -= view.UpdateStatsUI;
            model.OnDeath -= OnPlayerDeath;
            model.OnLifesaverAcquired -= () => view.ShowLifesaverIcon(true);
            model.OnLifesaverUsed -= () => view.ShowLifesaverIcon(false);
        }
    }

    public PlayerModel Model
    {
        get { return model; }
    }
}