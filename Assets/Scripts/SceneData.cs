using UnityEngine;

public class SceneData : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject[] allModals;
// В SceneData.cs
    private void Update()
    {
        bool anyActive = false;
        foreach (var panel in allModals)
        {
            if (panel != null && panel.activeSelf)
            {
                anyActive = true;
                break;
            }
        }

        // Если ни одна модалка не активна, проверяем, не открыт ли диалог
        // (Если DialogueManager.Instance.dialoguePanel.activeSelf)
        
        if (GameManager.Instance != null)
        {
            // Перезаписываем только если нашли активную панель
            // Или используй логическое ИЛИ, чтобы не затирать значение из других скриптов
            if (anyActive) GameManager.Instance.isAnyPanelOpen = true;
        }
    }
    private void Start()
    {
        if (GameManager.Instance != null)
        {
            // Передаем панель из этой сцены в глобальный менеджер
            GameManager.Instance.SetPausePanel(pausePanel);
        }
    }
}