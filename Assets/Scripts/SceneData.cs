using UnityEngine;

public class SceneData : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject[] allModals;

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            // Передаем панель из этой сцены в глобальный менеджер
            GameManager.Instance.SetPausePanel(pausePanel);
        }
    }
}