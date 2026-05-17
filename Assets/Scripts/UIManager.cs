using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private Stack<GameObject> _panelStack = new Stack<GameObject>();
    private bool _isPaused = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Вызывается, когда открывается любая модальная панель (диалог, магазин, прокачка)
    public void OpenPanel(GameObject panel)
    {
        if (panel == null) return;
        if (_panelStack.Contains(panel)) return; // уже открыта

        panel.SetActive(true);
        _panelStack.Push(panel);
        ApplyGameState();
    }

    // Закрывает верхнюю панель
    public void CloseTopPanel()
    {
        if (_panelStack.Count == 0) return;
        GameObject panel = _panelStack.Pop();
        panel.SetActive(false);
        ApplyGameState();
    }

    // Закрыть все панели (по Escape)
    public void CloseAllPanels()
    {
        while (_panelStack.Count > 0)
        {
            var panel = _panelStack.Pop();
            if (panel != null) panel.SetActive(false);
        }
        ApplyGameState();
    }

    // Вызывается из кнопки паузы (или по Escape)
    public void TogglePause()
    {
        if (_panelStack.Count > 0)
        {
            // Если есть открытые панели – сначала закроем их
            CloseAllPanels();
            return;
        }

        _isPaused = !_isPaused;
        ApplyGameState();

        // Показать/скрыть панель паузы (найти её в сцене или передать ссылку)
        var pausePanel = GameObject.FindGameObjectWithTag("PausePanel");
        if (pausePanel != null) pausePanel.SetActive(_isPaused);
    }

    private void ApplyGameState()
    {
        bool isBlocked = _isPaused || _panelStack.Count > 0;

        Time.timeScale = isBlocked ? 0f : 1f;

        // Управление курсором
        Cursor.lockState = isBlocked ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isBlocked;
    }

    // Проверить, заблокирована ли игра (для PlayerRuntime)
    public bool IsGameBlocked => _isPaused || _panelStack.Count > 0;
}