using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Settings")]
    public string menuScene = "MenuScene";
    
    private GameObject pausePanel;
    private bool isPaused = false;
    public PlayerModel PlayerModel { get; private set; }
    public PlayerData playerData;

    // Список для отслеживания всех открытых панелей (Магазин, Скиллы и т.д.)
    private List<GameObject> _activePanels = new List<GameObject>();
    [HideInInspector] public bool isAnyPanelOpen = false;

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject);
        PlayerModel = new PlayerModel(playerData);
        PlayerModel.OnDeath += RespawnPlayer; }
        else { Destroy(gameObject); }
    }
    public void RegisterPanel(GameObject panel) => _activePanels.Add(panel);
    public void UnregisterPanel(GameObject panel) => _activePanels.Remove(panel);

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == menuScene) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Если открыт магазин или прокачка — НЕ открываем паузу
            if (isAnyPanelOpen)
            {
                CloseAllUI();
            }
            else
            {
                TogglePause();
            }
        }
    }
    private void RespawnPlayer()
    {
        Debug.Log("Менеджер: Начинаем возрождение...");
        
        // 1. Полностью исцеляем героя перед перемещением
        PlayerModel.CurrentHealth = PlayerModel.MaxHealth;
        
        // 2. Сбрасываем флаги (паузы, открытые панели), если они были
        isPaused = false;
        isAnyPanelOpen = false;
        Time.timeScale = 1f;
        PlayerModel.ResetPerksState();
        // 3. Загружаем сцену Хаба
        // Убедись, что название сцены совпадает с тем, что в Build Settings
        SceneManager.LoadScene("HubScene"); 
    }
    private void CloseAllUI()
    {
        // Ищем все объекты со скриптом StatsUpgrader или Shop и закрываем их
        var upgraders = Object.FindObjectsByType<StatsUpgrader>(FindObjectsSortMode.None);
        foreach (var u in upgraders) u.ClosePanel();

        // Если у тебя есть отдельный скрипт для магазина, добавь его сюда так же
        // var shops = Object.FindObjectsByType<WeaponShop>(FindObjectsSortMode.None);
        // foreach (var s in shops) s.ClosePanel();

        isAnyPanelOpen = false;
        ResumeGameplayState();
    }
    public void StartGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        // Загружаем следующую сцену (убедись, что она добавлена в Build Settings)
        SceneManager.LoadScene("HubScene"); 
    }
    public void QuitGame()
    {
        Debug.Log("Игра закрывается...");
        Application.Quit();
    }
    public void BackToMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        _activePanels.Clear(); // Очищаем список панелей при выходе
        SceneManager.LoadScene(menuScene);
    }
    private void CloseLastPanel()
    {
        // Берем последнюю открытую панель
        GameObject panel = _activePanels[_activePanels.Count - 1];
        
        // Пытаемся закрыть через скрипт (чтобы вернулось время и т.д.)
        if (panel.TryGetComponent<StatsUpgrader>(out var upgrader))
        {
            upgrader.ClosePanel();
        }
        else
        {
            // Если скрипта нет, просто выключаем и удаляем из списка
            panel.SetActive(false);
            UnregisterPanel(panel);
            
            // Если это была последняя панель — возвращаем мир в норму
            if (_activePanels.Count == 0) ResumeGameplayState();
        }
    }

    public void TogglePause()
    {
        if (pausePanel == null) return;

        isPaused = !isPaused;
        pausePanel.SetActive(isPaused);
        
        // Обновляем состояние времени и курсора
        ApplyGameState();
    }

    public void ResumeGameplayState()
    {
        isPaused = false;
        // Мы не очищаем _activePanels здесь, так как этот метод 
        // вызывается и при закрытии магазина, когда панель уже удалена из списка
        ApplyGameState();
    }
    private void ApplyGameState()
    {
        // Игра считается "замороженной", если включена пауза ИЛИ открыта любая панель
        bool busy = isPaused || _activePanels.Count > 0;

        Time.timeScale = busy ? 0f : 1f;
    }

    public void SetPausePanel(GameObject panel) => pausePanel = panel;
}