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
            GameManager.Instance.SetPausePanel(pausePanel);
        }
    }
}