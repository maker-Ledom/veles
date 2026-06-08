using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    [Header("Settings")]
    public string targetSceneName = "LevelScene";
    
    public void Teleport()
    {
        Debug.Log("Телепортация в " + targetSceneName);
        Time.timeScale = 1f;
        FadeTransition.LoadSceneWithFade(targetSceneName);
    }
}