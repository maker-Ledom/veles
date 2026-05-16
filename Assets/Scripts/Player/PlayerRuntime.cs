using UnityEngine;

public class PlayerRuntime : MonoBehaviour
{
    [SerializeField] private PlayerData data;
    [SerializeField] private PlayerView view;
    
    private PlayerModel _model;
    public PlayerModel Model => _model;
    private PlayerController _controller;

    private void Awake()
    {
        // Вместо new PlayerModel(data) берем готовую из менеджера
        if (GameManager.Instance != null && GameManager.Instance.PlayerModel != null)
        {
            _model = GameManager.Instance.PlayerModel;
        }
        else
        {
            // На случай, если запускаем сцену минуя меню (для тестов)
            _model = new PlayerModel(data);
        }

        _controller = GetComponent<PlayerController>();
        _controller.Init(_model, data);

        // Подписки на UI остаются прежними
        _model.OnCoinsChanged += view.UpdateCoins;
        _model.OnWeaponChanged += view.UpdateWeaponIcon;
        _model.OnHealthChanged += view.UpdateHealth;
        _model.OnStatsChanged += view.UpdateStatsUI;
        _model.OnDeath += view.PlayDeathAnimation;
    }
    private void Start()
    {
        view.UpdateStatsUI(_model.Strength, _model.Agility);
        view.UpdateCoins(_model.Coins);
        view.SetupHealth(_model.MaxHealth);
        _model.CurrentHealth = _model.MaxHealth;
        view.UpdateHealth(_model.CurrentHealth, _model.MaxHealth);
        if (_model.CurrentWeapon != null)
        {
            view.UpdateWeaponIcon(_model.CurrentWeapon);
        }
    }
    private void OnDestroy()
    {
        // Важно отписаться при уничтожении объекта
        if (_model != null)
            _model.OnCoinsChanged -= view.UpdateCoins;
    }
    private void Update()
    {
// ПРОВЕРКА 1: Физическая пауза времени
    if (Time.timeScale <= 0.0001f) return; 

    // ПРОВЕРКА 2: Флаг панелей (на всякий случай)
    if (GameManager.Instance != null && GameManager.Instance.isAnyPanelOpen) return;
        // 1. Получаем «сырой» ввод WASD
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        Vector2 input = new Vector2(moveX, moveZ);

        // 2. Рассчитываем направления ОТНОСИТЕЛЬНО КАМЕРЫ
        // Берем вектор "вперед" камеры и убираем наклон по вертикали
        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0;
        camForward.Normalize();

        // Берем вектор "вправо" камеры (он всегда горизонтален)
        Vector3 camRight = Camera.main.transform.right;
        camRight.y = 0;
        camRight.Normalize();

        // Итоговый вектор движения: W/S по вертикали экрана, A/D по горизонтали
        Vector3 cameraRelativeDir = (camForward * input.y + camRight * input.x).normalized;

        // 3. Поворот к курсору (остается независимым и мгновенным)
        RotatePlayerToMouse();

        // 4. Логика Рывка и Движения
        if (Input.GetKeyDown(KeyCode.Space) && _controller.CanDash)
        {
            // Рывок идет строго по направлению, которое мы вычислили выше
            StartCoroutine(_controller.Dash(cameraRelativeDir));
        }
        else
        {
            _controller.HandleMovement(cameraRelativeDir);
        }

        // 5. Обновляем анимации
        UpdateAnimator(cameraRelativeDir);
        if (Input.GetMouseButton(0)) // Удерживаем кнопку для стрельбы (автомат/узи)
        {
            _controller.TryShoot();
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            view.ToggleStatsPanel(true);
        }
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            view.ToggleStatsPanel(false);
        }
        bool isMoving = cameraRelativeDir.sqrMagnitude > 0.01f;
        view.UpdateMoveAnimation(isMoving);
    }

    private void UpdateAnimator(Vector3 moveDir)
    {
        // Проверяем, есть ли движение
        bool isMoving = moveDir.sqrMagnitude > 0.01f;
        
        // ВЫЗЫВАЕМ метод анимации (убери // перед строчкой)
        view.UpdateMoveAnimation(isMoving); 
    }
    private void RotatePlayerToMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        // Создаем плоскость на высоте ТАЗА или ГРУДИ игрока (например, transform.position.y + 1f)
        // Это уберет визуальное смещение при взгляде под углом.
        float targetHeight = transform.position.y + 1f; 
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, targetHeight, 0));

        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 pointToLook = ray.GetPoint(rayDistance);
            
            // Визуальная отладка: в эдиторе ты увидишь красную линию к точке, куда смотрит игрок
            Debug.DrawLine(transform.position, pointToLook, Color.red);

            _controller.HandleLookRotation(pointToLook);
        }
    }
    public void ApplyDamage(float damage)
    {
        _model.TakeDamage(damage); // Модель сама вызовет OnHealthChanged
    }
// В файле PlayerRuntime.cs обнови метод:
    public void CollectCoin(int amount)
    {
        _model.AddCoins(amount);
        // Здесь можно добавить звук подбора через PlayerView
    }
    private void OnDisable()
    {
        if (_model != null)
        {
            // ОТПИСЫВАЕМСЯ от событий, чтобы модель не вызывала методы у мертвого UI
            _model.OnCoinsChanged -= view.UpdateCoins;
            _model.OnWeaponChanged -= view.UpdateWeaponIcon;
            _model.OnHealthChanged -= view.UpdateHealth;
            _model.OnStatsChanged -= view.UpdateStatsUI;
            _model.OnDeath -= view.PlayDeathAnimation;
        }
    }
}