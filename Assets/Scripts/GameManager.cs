using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Settings")]
    public string menuSceneName = "MenuScene";
    public PlayerData playerData;

    private PlayerModel playerModel;
    private GameObject pausePanelReference;
    private Stack<GameObject> activePanelStack = new Stack<GameObject>();
    private bool isGamePaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            playerModel = new PlayerModel(playerData);
            playerModel.OnDeath += RespawnPlayer;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            if (FindObjectOfType<FadeTransition>() == null)
            {
                GameObject fadePrefab = Resources.Load<GameObject>("FadeTransitionPrefab");
                if (fadePrefab != null) Instantiate(fadePrefab);
                else Debug.LogError("Не найден префаб FadeTransitionPrefab в папке Resources");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == menuSceneName) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (activePanelStack.Count > 0)
            {
                CloseAllPanels();
            }
            else
            {
                TogglePause();
            }
        }
    }

    public void OpenPanel(GameObject panel)
    {
        Debug.Log($"OpenPanel called with {panel?.name}. Stack size before: {activePanelStack.Count}");
        if (panel == null) return;
        if (activePanelStack.Contains(panel)) return;

        panel.SetActive(true);
        activePanelStack.Push(panel);
        ApplyGameBlockState();
    }

    public void CloseTopPanel()
    {
        if (activePanelStack.Count == 0)
        {
            Debug.LogWarning("CloseTopPanel called but stack is empty");
            return;
        }
        
        GameObject panel = activePanelStack.Pop();
        Debug.Log($"CloseTopPanel: closing panel '{panel?.name}'. Remaining stack size: {activePanelStack.Count}");
        
        if (panel != null)
            panel.SetActive(false);
        else
            Debug.LogError("Panel object is null in stack!");
        
        ApplyGameBlockState();
    }

    public void CloseAllPanels()
    {
        while (activePanelStack.Count > 0)
        {
            GameObject panel = activePanelStack.Pop();
            if (panel != null) panel.SetActive(false);
        }
        ApplyGameBlockState();
    }

    public void TogglePause()
    {
        if (activePanelStack.Count > 0)
        {
            CloseAllPanels();
            return;
        }

        isGamePaused = !isGamePaused;
        ApplyGameBlockState();

        if (pausePanelReference != null)
        {
            pausePanelReference.SetActive(isGamePaused);
        }
    }

    private void ApplyGameBlockState()
    {
        bool isBlocked = isGamePaused || activePanelStack.Count > 0;
        Time.timeScale = isBlocked ? 0f : 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public bool IsGameBlocked()
    {
        return isGamePaused || activePanelStack.Count > 0;
    }

    public void SetPausePanel(GameObject panel)
    {
        pausePanelReference = panel;
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        isGamePaused = false;
        CloseAllPanels();
        FadeTransition.LoadSceneWithFade("HubScene");
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game");
        Application.Quit();
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        isGamePaused = false;
        activePanelStack.Clear();
        FadeTransition.LoadSceneWithFade(menuSceneName);
    }

    private void RespawnPlayer()
    {
        Debug.Log("Respawning player to HubScene");

        playerModel.CurrentHealth = playerModel.MaxHealth;
        isGamePaused = false;
        Time.timeScale = 1f;
        playerModel.ResetPerksState();

        FadeTransition.LoadSceneWithFade("HubScene");
    }

    public PlayerModel PlayerModel
    {
        get { return playerModel; }
    }
}