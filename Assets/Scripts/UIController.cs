using UnityEngine;

public class UIController : MonoBehaviour
{
    // Методы для кнопок меню
    public void OnStartClicked() => GameManager.Instance.StartGame();
    public void OnExitClicked() => GameManager.Instance.QuitGame();

    // Методы для панели паузы
    public void OnResumeClicked() => GameManager.Instance.TogglePause();
    public void OnMenuClicked() => GameManager.Instance.BackToMenu();
}