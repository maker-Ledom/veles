using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [Header("Settings")]
    public string targetSceneName = "LevelScene";
    
    // Этот метод мы привяжем к событию OnInteract в инспекторе
    public void Teleport()
    {
        Debug.Log("Телепортация в " + targetSceneName);
        
        // Перед переходом важно убедиться, что время идет (если портал открылся из меню)
        Time.timeScale = 1f;
        
        // Загружаем новую сцену
        SceneManager.LoadScene(targetSceneName);
    }
}