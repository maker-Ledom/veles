using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeTransition : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 0.8f;
    private static FadeTransition instance;
    private Image fadeImage;
    private Canvas fadeCanvas;
    private bool isFading = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        // СОЗДАЁМ КАНВАС И ИЗОБРАЖЕНИЕ ПРОГРАММНО
        CreateFadeCanvas();
    }

    private void CreateFadeCanvas()
    {
        // Создаём GameObject для Canvas
        GameObject canvasObj = new GameObject("FadeCanvas");
        canvasObj.transform.SetParent(transform); // делаем дочерним для FadeTransition
        fadeCanvas = canvasObj.AddComponent<Canvas>();
        fadeCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        fadeCanvas.sortingOrder = 30000; // Очень высокий порядок, чтобы быть поверх всего
        
        // Добавляем CanvasScaler для корректного растягивания
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        // Добавляем GraphicRaycaster (не обязательно, но для полноты)
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Создаём Image
        GameObject imageObj = new GameObject("FadeImage");
        imageObj.transform.SetParent(canvasObj.transform);
        RectTransform rect = imageObj.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        fadeImage = imageObj.AddComponent<Image>();
        fadeImage.color = new Color(0, 0, 0, 0); // полностью прозрачный чёрный
        
        // Изначально Canvas выключен
        fadeCanvas.gameObject.SetActive(false);
    }

    public static void LoadSceneWithFade(string sceneName, System.Action onComplete = null)
    {
        if (instance == null)
        {
            Debug.LogError("FadeTransition не найден! Сцена загрузится без затухания.");
            SceneManager.LoadScene(sceneName);
            onComplete?.Invoke();
            return;
        }
        instance.StartCoroutine(instance.FadeAndLoad(sceneName, onComplete));
    }

    private IEnumerator FadeAndLoad(string sceneName, System.Action onComplete)
    {
        if (isFading) yield break;
        isFading = true;

        fadeCanvas.gameObject.SetActive(true);
        yield return StartCoroutine(Fade(0f, 1f));

        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = true;
        while (!async.isDone)
            yield return null;

        // Небольшая задержка, чтобы новая сцена успела активироваться
        yield return new WaitForEndOfFrame();

        yield return StartCoroutine(Fade(1f, 0f));
        fadeCanvas.gameObject.SetActive(false);

        isFading = false;
        onComplete?.Invoke();
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        Color color = fadeImage.color;
        while (elapsed < fadeDuration)
        {
            float t = elapsed / fadeDuration;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            color.a = alpha;
            fadeImage.color = color;
            elapsed += Time.deltaTime;
            yield return null;
        }
        color.a = endAlpha;
        fadeImage.color = color;
    }
}