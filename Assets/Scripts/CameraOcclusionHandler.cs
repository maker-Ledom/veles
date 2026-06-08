using UnityEngine;
using System.Collections.Generic;

public class CameraOcclusionHandler : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target; // Ссылка на персонажа (Target)
    [SerializeField] private LayerMask wallLayer; // Слой, на котором находятся стены

    [Header("Transparency Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float transparentAlpha = 0.3f; // Насколько прозрачной станет стена
    [SerializeField] private float fadeSpeed = 5f; // Скорость исчезновения/появления

    private List<OccludedObject> currentlyOccluded = new List<OccludedObject>();
    private List<OccludedObject> toRemove = new List<OccludedObject>();

    void LateUpdate()
    {
        if (target == null) return;

        // Сбрасываем старые объекты, которые больше не загораживают обзор
        ClearOldOcclusions();

        // Проверяем, есть ли преграда между камерой и персонажем
        Vector3 direction = target.position - transform.position;
        float distance = direction.magnitude;
        RaycastHit[] hits = Physics.RaycastAll(transform.position, direction, distance, wallLayer);

        foreach (RaycastHit hit in hits)
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();
            if (renderer != null)
            {
                ManageOcclusion(renderer);
            }
        }
    }

    private void ManageOcclusion(Renderer renderer)
    {
        // Проверяем, обрабатывается ли уже этот объект
        OccludedObject existing = currentlyOccluded.Find(x => x.renderer == renderer);

        if (existing == null)
        {
            OccludedObject newOcclusion = new OccludedObject(renderer, transparentAlpha, fadeSpeed);
            currentlyOccluded.Add(newOcclusion);
        }
        else
        {
            existing.KeepOccluded();
        }
    }

    private void ClearOldOcclusions()
    {
        toRemove.Clear();

        foreach (var occluded in currentlyOccluded)
        {
            if (!occluded.isStillBlocking)
            {
                occluded.Restore();
                if (occluded.IsFullyRestored())
                {
                    toRemove.Add(occluded);
                }
            }
            else
            {
                occluded.FadeOut();
            }
        }

        foreach (var remove in toRemove)
        {
            currentlyOccluded.Remove(remove);
        }
    }
}

// Вспомогательный класс для управления материалами конкретного объекта
public class OccludedObject
{
    public Renderer renderer;
    private Material[] originalMaterials;
    private Material[] originalMaterialsCopy; // Копия для плавной смены альфы
    private float targetAlpha;
    private float currentAlpha = 1f;
    private float fadeSpeed;
    public bool isStillBlocking;

    public OccludedObject(Renderer renderer, float targetAlpha, float fadeSpeed)
    {
        this.renderer = renderer;
        this.targetAlpha = targetAlpha;
        this.fadeSpeed = fadeSpeed;
        this.originalMaterials = renderer.sharedMaterials;
        
        // Создаем копии материалов и переключаем их в режим Transparent/Fade
        this.originalMaterialsCopy = renderer.materials; 
        foreach (var mat in originalMaterialsCopy)
        {
            SetupTransparentMode(mat);
        }
        
        isStillBlocking = true;
    }

    public void KeepOccluded()
    {
        isStillBlocking = true;
    }

    public void FadeOut()
    {
        currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, Time.deltaTime * fadeSpeed);
        SetAlpha(currentAlpha);
        isStillBlocking = false; // Сбрасываем флаг для следующего кадра
    }

    public void Restore()
    {
        currentAlpha = Mathf.MoveTowards(currentAlpha, 1f, Time.deltaTime * fadeSpeed);
        SetAlpha(currentAlpha);
    }

    public bool IsFullyRestored()
    {
        if (currentAlpha >= 0.99f)
        {
            renderer.materials = originalMaterials; // Возвращаем исходные материалы (Opaque)
            return true;
        }
        return false;
    }

    private void SetAlpha(float alpha)
    {
        foreach (var mat in originalMaterialsCopy)
        {
            if (mat.HasProperty("_Color"))
            {
                Color c = mat.color;
                c.a = alpha;
                mat.color = c;
            }
        }
    }

    // Настройка стандартного шейдера Unity (Standard Shader) на прозрачность
    private void SetupTransparentMode(Material material)
    {
        material.SetFloat("_Mode", 2); // 2 — это Fade, 3 — Transparent
        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        material.SetInt("_ZWrite", 0);
        material.DisableKeyword("_ALPHATEST_ON");
        material.EnableKeyword("_ALPHABLEND_ON");
        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        material.renderQueue = 3000;
    }
}